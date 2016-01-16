using System;
using System.Linq;

namespace OTA.Config
{
    /// <summary>
    /// Key value register.
    /// </summary>
    public class PairFileRegister : DataRegister
    {
        private bool _lowerKeys;

        const Char PrefixKey = '=';

        public PairFileRegister(string path, bool lowerKeys = true, bool autoLoad = true) : base(path, autoLoad)
        {
            _lowerKeys = lowerKeys;
        }

        public override void Load()
        {
            lock (_data)
            {
                if (System.IO.File.Exists(_path))
                {
                    _data = System.IO.File.ReadAllLines(_path)
                        .Select(x => x.Trim())
                        .Distinct()
                        .ToArray();

                    //Some lines may have the key separate.
                    for (var i = 0; i < _data.Length; i++)
                    {
                        var ix = _data[i].IndexOf(PrefixKey);
                        if (ix > -1)
                        {
                            var key = _data[i].Substring(0, ix).Trim();
                            //Don't trim the value, as it may be a string
                            var value = _data[i].Remove(0, ix + 1);
                            _data[i] = key + PrefixKey + value;
                        }
                        else
                        {
                            //Leave line alone.
                            continue;
                        }
                    }
                }
                else
                    System.IO.File.WriteAllText(_path, System.String.Empty);
            }
        }

        /// <summary>
        /// Add the specified key and value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        public bool Add(string key, string value, bool autoSave = true)
        {
            lock (_data)
            {
                System.Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = (_lowerKeys ? key.ToLower() : key).Trim() + PrefixKey + value;
            }

            if (autoSave)
                return Save();
            return true;
        }

        /// <summary>
        /// Update a keys value
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        public bool Update(string key, string value, bool autoSave = true)
        {
            key = (_lowerKeys ? key.ToLower() : key).Trim();
            bool updated = false;
            lock (_data)
            {
                for (var x = 0; x < _data.Length; x++)
                {
                    if ((_lowerKeys ? _data[x].ToLower() : _data[x]).StartsWith(key + PrefixKey))
                    {
                        updated = true;
                        _data[x] = key + PrefixKey + (value ?? String.Empty);
                    }
                }
            }

            if (!updated)
            {
                updated = Add(key, value, autoSave);
            }

            if (autoSave)
                return Save() && updated;
            return updated;
        }

        /// <summary>
        /// Retreives a listing by the key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Find(string key)
        {
            var cleaned = (_lowerKeys ? key.ToLower() : key).Trim();
            var item = _data
                .Where(x => (_lowerKeys ? x.ToLower() : x).StartsWith(cleaned + PrefixKey))
                .ToArray();

            if (item.Length == 1)
            {
                var v = item[0].Remove(0, item[0].IndexOf(PrefixKey) + 1);
                if (!String.IsNullOrEmpty(v))
                    return v;
            }

            return null;
        }

        /// <summary>
        /// Determine if a key and value exist
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="compareAsLowered">If set to <c>true</c> compare as lowered.</param>
        public bool Contains(string key, string value, bool compareAsLowered = false)
        {
            return base.Contains(key + PrefixKey + value, compareAsLowered);
        }
    }
}