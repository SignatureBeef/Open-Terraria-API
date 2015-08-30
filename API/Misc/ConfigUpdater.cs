using System;

namespace OTA
{
    /// <summary>
    /// A generic configuration updater for the format of the official serverconfig.txt.
    /// </summary>
    public static class ConfigUpdater
    {
        internal static string SourceFile { get; set; }

        /// <summary>
        /// Gets a value indicating is available.
        /// </summary>
        /// <remarks>If a configuration file is not specified to the program we cannot update.</remarks>
        /// <value><c>true</c> if is available; otherwise, <c>false</c>.</value>
        public static bool IsAvailable
        {
            get
            { return !String.IsNullOrEmpty(SourceFile); }
        }

        static object _lock = new object();

        /// <summary>
        /// Update the specified configuration file with the prefix.
        /// </summary>
        /// <param name="key">Prefix to use. E.g. motd</param>
        /// <param name="value">Value to be set</param>
        /// <param name="includeCommented">If set to <c>true</c>, allows commented lines to be updated.</param>
        /// <param name="preserveComment">If set to <c>true</c> the save will preserve commented lines.</param>
        public static bool Set(string key, string value, bool includeCommented = true, bool preserveComment = false)
        {
            if (!IsAvailable) throw new InvalidOperationException("No config file was specified");

            bool changed = false;
            lock (_lock)
            {
                var lines = System.IO.File.ReadAllLines(SourceFile);

                //Add the separator
                key += '=';

                string commented = '#' + key;
                for (var x = 0; x < lines.Length; x++)
                {
                    var line = lines[x];

                    if (line.StartsWith(key))
                    {
                        lines[x] = key + value;
                        changed = true;
                    }
                    else if (includeCommented && line.StartsWith(commented))
                    {
                        if (preserveComment) lines[x] = commented + value;
                        else lines[x] = key + value;
                        changed = true;
                    }
                }

                if (changed)
                {
                    System.IO.File.WriteAllLines(SourceFile, lines);
                }

                if (lines.Length == 0) return true;
            }

            return changed;
        }

        /// <summary>
        /// Set the specified key, value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="includeCommented">If set to <c>true</c> include commented.</param>
        /// <param name="preserveComment">If set to <c>true</c> preserve comment.</param>
        public static bool Set(string key, object value, bool includeCommented = true, bool preserveComment = false)
        {
            return Set(key, value.ToString(), includeCommented, preserveComment);
        }
    }
}

