using System;
using System.IO;

namespace LuaApi
{
    [OTAPI.Modification(OTAPI.ModType.Runtime, "Loading LUA interface")]
    public class LuaApi
    {
        public Triton.Lua runtime;

        public LuaApi()
        {
            System.Console.WriteLine($"[LUA] Starting runtime");
            runtime = new Triton.Lua();

            LoadPlugins();
        }

        void LoadPlugins()
        {
            if (Directory.Exists("lua"))
            {
                foreach (var file in Directory.EnumerateFiles("lua", "*.lua", SearchOption.AllDirectories))
                {
                    System.Console.WriteLine($"[LUA] Loading plugin: {file}");
                    try
                    {
                        var contents = File.ReadAllText(file);
                        runtime.DoString(contents);
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine($"[LUA] Load error: {ex}");
                    }
                }
            }
        }
    }
}
