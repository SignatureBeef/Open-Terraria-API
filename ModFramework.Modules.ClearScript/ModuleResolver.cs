using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ModFramework.Modules.ClearScript
{
    //class ImportHostObject : Document
    //{
    //    protected DocumentInfo info;
    //    protected Stream contents;

    //    public ImportHostObject(DocumentCategory category, string name)
    //    {
    //        this.info = new DocumentInfo(name)
    //        {
    //            Category = category,
    //        };
    //        this.contents = new MemoryStream(System.Text.Encoding.UTF8.GetBytes($"export default {name};"));
    //    }

    //    public override DocumentInfo Info => info;
    //    public override Stream Contents => contents;
    //}

    public class ModuleResolver : DocumentLoader, IDisposable
    {
        protected V8ScriptEngine engine;
        protected DocumentLoader root;

        private Dictionary<string, Document> cache = new Dictionary<string, Document>();

        public ModuleResolver(V8ScriptEngine engine, DocumentLoader root)
        {
            this.engine = engine;
            this.root = root;
        }

        public StringDocument AddDocument(string name, string code, DocumentCategory category)
        {
            var doc = new StringDocument(new DocumentInfo(name) { Category = category }, code);
            cache[name] = doc;
            return doc;
        }

        public void Unload(string name)
        {
            if (cache.ContainsKey(name))
                cache.Remove(name);
        }

        const string Prefix_AddHostObject = "AddHostObject-";
        const string Prefix_AddHostType = "AddHostType-";

        static string Clean(string name)
        {
            return name.Replace(".", "_").Replace("-", "_").Replace(",", "_");
        }

        public override Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
        {
            var module_name = Clean(specifier);

            if (cache.TryGetValue(module_name, out Document cached) && cached != null)
            {
                //var sw = new StreamReader(cached.Contents);
                //var txt = sw.ReadToEnd();
                return Task.FromResult(cached);
            }

            if (specifier.StartsWith(Prefix_AddHostObject, StringComparison.CurrentCultureIgnoreCase))
            {
                var csv = specifier.Substring(Prefix_AddHostObject.Length);
                engine.AddHostObject(module_name, new HostTypeCollection(csv.Split(',')));
                return Task.FromResult<Document>(AddDocument(module_name, $"export default {module_name};", category));
            }

            if (specifier.StartsWith(Prefix_AddHostType, StringComparison.CurrentCultureIgnoreCase))
            {
                var type = specifier.Substring(Prefix_AddHostType.Length);
                engine.AddHostType(module_name, type);
                return Task.FromResult<Document>(AddDocument(module_name, $"export default {module_name};", category));
            }

            return root.LoadDocumentAsync(settings, sourceInfo, specifier, category, contextCallback);
        }

        public override void DiscardCachedDocuments()
        {
            base.DiscardCachedDocuments();
            cache.Clear();
        }

        public void Dispose()
        {
            cache?.Clear();
            cache = null;
        }
    }
}
