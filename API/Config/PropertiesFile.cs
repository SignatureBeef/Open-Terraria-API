using System;

namespace OTA.Config
{
    /// <summary>
    /// A basic low use key/value properties file implementation
    /// </summary>
    public class PropertiesFile : PairFileRegister
    {
        //public PropertiesFile(string path)
        //    : base(path)
        //{

        //}
        public PropertiesFile(string path, bool lowerKeys = true, bool autoLoad = true)
            : base(path, lowerKeys, autoLoad)
        {

        }

        /// <summary>
        /// Parses the value of an item when found using the key.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="autoSave">If set to <c>true</c> auto save.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetValue<T>(string key, T defaultValue, bool autoSave = false)
        {
            var item = base.Find(key);
            if (item != null)
            {
                Type t = typeof(T);
                if (t.Name == "Int16")
                {
                    short v = 0;
                    if (Int16.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "Int32")
                {
                    int v = 0;
                    if (Int32.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "Int64")
                {
                    long v = 0;
                    if (Int64.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "Double")
                {
                    double v = 0;
                    if (Double.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "Single")
                {
                    float v = 0;
                    if (Single.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "Boolean")
                {
                    bool v = false;
                    if (Boolean.TryParse(item.Trim(), out v))
                        return (T)(object)v;
                }
                else if (t.Name == "String")
                {
                    return (T)(object)item;
                }
            }
            else
            {
                base.Update(key, defaultValue.ToString(), autoSave);
            }
            return defaultValue;
        }

        public bool SetValue(string key, object value, bool autoSave = false)
        {
            return base.Update(key, value.ToString(), autoSave);
        }
    }
}

