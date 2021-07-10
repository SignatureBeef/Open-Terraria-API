using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModFramework
{
    public class ModificationMdDocumentor : MarkdownDocumentor
    {
        protected override bool CanWrite(bool header, ref string line)
        {
            if (line.Contains("@doc"))
            {
                line = line.Replace("@doc", "").Trim();
                return true;
            }
            return header;
        }
    }

    [MonoMod.MonoModIgnore]
    public class BasicComment
    {
        public string Comments { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }

        static BasicComment()
        {
            if (!MarkdownDocumentor.TypeFormatterDefined<BasicComment>())
            {
                const string HeaderFormat = $"| Type | Module | Comment |\n| ---- | ---- | ---- |";
                MarkdownDocumentor.RegisterTypeFormatter<BasicComment>((header, data) => header ?
                    HeaderFormat
                    :
                    $"| {data.Type} | [{Path.GetFileName(data.FilePath)}]({data.FilePath}) | {data?.Comments } |"
                );
            }
        }
    }

    public class MarkdownDocumentor : IDisposable
    {
        private Dictionary<Type, List<object>> data = new Dictionary<Type, List<object>>();

        public delegate string TypeFormatter<TRecord>(bool header, TRecord record);

        private static Dictionary<Type, TypeFormatter<object>> _typeFormatters = new Dictionary<Type, TypeFormatter<object>>();

        public delegate void AddRecordHandler<TRecord>(TRecord record, ref bool handled);
        public event AddRecordHandler<object> AddRecord;

        public delegate void WriteLineHandler(bool header, ref string input, ref bool handled);
        public event WriteLineHandler WriteLine;

        public static bool TypeFormatterDefined<TType>() => _typeFormatters.ContainsKey(typeof(TType));
        public static void RegisterTypeFormatter<TType>(TypeFormatter<TType> handler)
        {
            _typeFormatters[typeof(TType)] = (header, row) =>
            {
                return handler(header, (TType)row);
            };
        }

        TypeFormatter<object> GetTypeFormatter(Type type) => _typeFormatters[type];

        protected virtual bool CanWrite(bool header, ref string line)
        {
            var handled = false;
            WriteLine?.Invoke(header, ref line, ref handled);

            return !handled;
        }

        protected virtual bool CanAdd<TRecord>(TRecord record)
        {
            var handled = false;
            AddRecord?.Invoke(record, ref handled);

            return !handled;
        }

        public void Add<TMetaData>(TMetaData meta)
        {
            if (!CanAdd(meta)) return;

            var type = typeof(TMetaData);
            if (!data.TryGetValue(type, out List<object> metas))
            {
                metas = new List<object>();
                data[type] = metas;
            }

            metas.Add(meta);
        }

        public IEnumerable<TMetaData> Find<TMetaData>(Func<TMetaData, bool> condition)
        {
            var type = typeof(TMetaData);
            if (data.TryGetValue(type, out List<object> metas))
            {
                return metas.Select(x => (TMetaData)x).Where((data) => condition(data));
            }
            return Enumerable.Empty<TMetaData>();
        }

        public void Write(string filename)
        {
            using var writer = new StreamWriter(filename);

            foreach (var pair in data)
            {
                var formatter = GetTypeFormatter(pair.Key);

                // header
                var line = formatter(true, null);
                if (CanWrite(true, ref line))
                    writer.WriteLine(line);

                // data
                foreach (var item in pair.Value)
                {
                    line = formatter(false, item);
                    if (CanWrite(false, ref line))
                        writer.WriteLine(line);
                }

                writer.WriteLine();
            }

            writer.Flush();
            writer.Close();
        }

        public void Dispose()
        {
            var dict = data;
            if (dict != null)
            {
                foreach (var item in dict)
                    item.Value.Clear();

                dict.Clear();
            }
            data = null;
        }
    }
}
