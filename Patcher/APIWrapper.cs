

using System;
using System.Reflection;

namespace OTA.Patcher
{
    [Serializable]
    /// <summary>
    /// AppDomain proxy for accessing OTA
    /// </summary>
    public class Proxy : MarshalByRefObject
    {
        Assembly _api;

        /// <summary>
        /// Get the supported Terraria version
        /// </summary>
        /// <value>The terraria version.</value>
        public string TerrariaVersion
        {
            get
            {
                return (string)_api.GetType("OTA.Globals").GetField("TerrariaVersion").GetValue(null);
            }
        }

        /// <summary>
        /// Load an assembly into the domain
        /// </summary>
        /// <param name="path">Path.</param>
        public void Load(string path)
        {
            _api = Assembly.LoadFile(path);
        }
    }

    /// <summary>
    /// This class is to isolate and manage the tdsm.exe referenced by the api dll.
    /// Previously windows would lock the tdsm.exe that was referenced by TDSM.API.dll, which itself was loaded by the patcher.
    /// The locking would cause the patcher (if ran a second time) to fail when saving tdsm.exe
    /// </summary>

    public static class APIWrapper
    {
        //static Assembly _api;
        static Proxy _api;
        static AppDomain _domain;

        static APIWrapper()
        {
            _domain = AppDomain.CreateDomain("OPEN_TERRARIA_API_WRAPPER", null /*AppDomain.CurrentDomain.Evidence*/, new AppDomainSetup()
                {
                    //ShadowCopyFiles = "false",
                    ApplicationBase = Environment.CurrentDirectory/*, Commented out as OSX does not have this?
                    AppDomainManagerAssembly = String.Empty*/
                });

            var type = typeof(Proxy);
            foreach (var file in new string[] { "Patcher.exe", "OTA.dll" })
            {
                if (!System.IO.File.Exists(file))
                {
                    var bin = System.IO.Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", file);
                    if (System.IO.File.Exists(bin))
                    {
                        System.IO.File.Copy(bin, file);
                        Console.WriteLine("Copied: " + file);
                    }
                }
            }
            var plugin = _domain.CreateInstance(type.Assembly.FullName, type.FullName);
            _api = plugin.Unwrap() as Proxy;

            _api.Load(System.IO.Path.Combine(Environment.CurrentDirectory, "OTA.dll"));
        }

        /// <summary>
        /// Gets the supported Terraria version for OTA.
        /// </summary>
        /// <value>The terraria version.</value>
        public static string TerrariaVersion
        {
            get
            {
                return _api.TerrariaVersion;
            }
        }
    }
}

