using System;
using System.Collections;
using System.Linq;
using OTA.Logging;
using System.Collections.Generic;

namespace OTA.Config
{
    /// <summary>
    /// Low use data storage
    /// </summary>
    public class DataRegister : IEnumerator<string>
    {
        protected string _path;
        protected string[] _data;

        public DataRegister(string path, bool lowerKeys = true, bool autoLoad = true)
        {
            _path = path;
            _data = new string[0];

            if (autoLoad)
                Load();
        }

        /// <summary>
        /// Total registered items
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            { return _data.Length; }
        }

        /// <summary>
        /// Loads lines from the file
        /// </summary>
        public virtual void Load()
        {
            lock (_data)
            {
                if (System.IO.File.Exists(_path))
                {
                    _data = System.IO.File.ReadAllLines(_path)
                        .Select(x => x.Trim())
                        .Distinct()
                        .ToArray();
                }
                else
                    System.IO.File.WriteAllText(_path, System.String.Empty);
            }
        }

        /// <summary>
        /// Saves this instance into a file
        /// </summary>
        public virtual bool Save()
        {
            try
            {
                lock (_data)
                {
                    System.IO.File.WriteAllLines(_path,
                        _data.Distinct().ToArray()
                    );
                }
                return true;
            }
            catch (System.Exception e)
            {
                Logger.Log(e, "Failure saving data list.");
                return false;
            }
        }

        /// <summary>
        /// Clear out this instance
        /// </summary>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        public virtual bool Clear(bool autoSave = true)
        {
            lock (_data)
            {
                _data = new string[] { };
            }

            if (autoSave)
                return Save();
            return true;
        }

        /// <summary>
        /// Adds a new line into this instance
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        public virtual bool Add(string item, bool autoSave = true)
        {
            lock (_data)
            {
                System.Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = item;
            }

            if (autoSave)
                return Save();
            return true;
        }

        /// <summary>
        /// Removes a line from this instance
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        public virtual bool Remove(string item, bool autoSave = true)
        {
            lock (_data)
            {
                _data = _data.Where(x => x != item).ToArray();
            }

            if (autoSave)
                return Save();
            return true;
        }

        /// <summary>
        /// Checks to see if this instance contains a line
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="compareAsLowered">If set to <c>true</c> compare as lowered.</param>
        public virtual bool Contains(string item, bool compareAsLowered = false)
        {
            lock (_data)
            {
                if (compareAsLowered) return _data.Contains(item.ToLower());
                return _data.Contains(item);
            }
        }

        //        public bool Remove(string item, bool byKey = false, bool autoSave = true)
        //        {
        //            var cleaned = (_lowerKeys || !byKey ? item.ToLower() : item).Trim();
        //            lock (_data)
        //            {
        //                if (byKey)
        //                    _data = _data.Where(x => !(_lowerKeys ? x.ToLower() : x).StartsWith(cleaned + PrefixKey)).ToArray();
        //                else
        //                    _data = _data.Where(x => x != cleaned).ToArray();
        //            }
        //
        //            if (autoSave)
        //                return Save();
        //            return true;
        //        }
        //
        //        public bool Contains(string item)
        //        {
        //            var cleaned = (_lowerKeys || !byKey ? item.ToLower() : item).ToLower().Trim();
        //            lock (_data)
        //            {
        //                if (byKey)
        //                    return _data.Where(x => x.StartsWith(cleaned + PrefixKey)).Count() > 0;
        //                else
        //                    return _data.Where(x => x == item).Count() > 0;
        //            }
        //        }
        //
        //        public bool Contains(string key, string value)
        //        {
        //            var cleaned = (_lowerKeys ? key.ToLower() : key).Trim();
        //            lock (_data)
        //            {
        //                return _data.Where(x => x == (cleaned + PrefixKey + value)).Count() > 0;
        //            }
        //        }

        public string this [int index]
        {
            get
            { return _data[index]; }
        }

        private int _index;

        public string Current
        {
            get { return _data[_index]; }
        }

        object IEnumerator.Current
        {
            get { return _data[_index]; }
        }

        public bool MoveNext()
        {
            return (++_index) < _data.Length;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            _data = null;
            _index = 0;
            _path = null;
        }
    }
}