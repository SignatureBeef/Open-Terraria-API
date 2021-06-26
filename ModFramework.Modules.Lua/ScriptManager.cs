/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLua.Exceptions;

namespace ModFramework.Modules.Lua
{
    public delegate bool FileFoundHandler(string filepath);

    class LuaScript : IDisposable
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public NLua.Lua Container { get; set; }
        public string Content { get; set; }

        public object[] LoadResult { get; set; }
        public object LoadError { get; set; }

        public ScriptManager Manager { get; set; }

        public LuaScript(ScriptManager manager)
        {
            Manager = manager;
        }

        public void UnloadLua()
        {
            var dispose = Container["Dispose"] as NLua.LuaFunction;
            if (dispose != null)
            {
                dispose.Call();
            }
        }

        public void Dispose()
        {
            if (Container != null) UnloadLua();
            Container?.Dispose();
            FilePath = null;
            FileName = null;
            Container = null;
            Content = null;
            LoadResult = null;
            LoadError = null;
        }

        public IEnumerable<string> GetComments()
        {
            return Content.Split('\n')
                .Where(line => (line.Trim().StartsWith("-- @doc", StringComparison.CurrentCultureIgnoreCase)))
                .Select(line => line.Replace("-- ", "").Trim());
        }

        void RegisterComments()
        {
            var doc = Manager.GetMarkdownDocumentor();
            if (doc is not null)
            {
                var comments = this.GetComments();
                if (comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        doc.Add(new BasicComment()
                        {
                            Comments = comment,
                            Type = "script",
                            FilePath = this.FilePath,
                        });
                    }
                }
            }
        }

        public void Load()
        {
            try
            {
                if (Container != null) UnloadLua();
                Container?.Dispose();

                Container = new NLua.Lua();
                Container.LoadCLRPackage();

                if (Manager.Modder != null)
                    Container["Modder"] = Manager.Modder;

                Content = File.ReadAllText(FilePath);
                LoadResult = Container.DoString(Content);

                RegisterComments();
            }
            catch (LuaScriptException ex)
            {
                LoadError = ex;
                Console.WriteLine("[Lua] Load failed");
                Console.WriteLine(ex);
                Console.WriteLine(ex.InnerException);
            }
            catch (Exception ex)
            {
                LoadError = ex;
                Console.WriteLine("[Lua] Load failed");
                Console.WriteLine(ex);
            }
        }
    }

    public class ScriptManager : IDisposable
    {
        public string ScriptFolder { get; set; }

        public static event FileFoundHandler FileFound;

        private List<LuaScript> _scripts { get; } = new List<LuaScript>();
        private FileSystemWatcher _watcher { get; set; }

        public ModFwModder Modder { get; set; }

        public MarkdownDocumentor MarkdownDocumentor { get; set; }

        public ScriptManager(
            string scriptFolder,
            ModFwModder modder
        )
        {
            ScriptFolder = scriptFolder;
            Modder = modder;
        }

        public ScriptManager SetMarkdownDocumentor(MarkdownDocumentor documentor)
        {
            MarkdownDocumentor = documentor;
            return this;
        }

        public MarkdownDocumentor GetMarkdownDocumentor()
        {
            if (MarkdownDocumentor is not null)
                return MarkdownDocumentor;

            return Modder?.MarkdownDocumentor;
        }

        LuaScript CreateScriptFromFile(string file)
        {
            Console.WriteLine($"[LUA] Loading {file}");

            var script = new LuaScript(this)
            {
                FilePath = file,
                FileName = Path.GetFileNameWithoutExtension(file),
            };

            _scripts.Add(script);

            script.Load();

            return script;
        }

        public void Initialise()
        {
            var scripts = Directory.GetFiles(ScriptFolder, "*.lua");
            foreach (var file in scripts)
            {
                if (FileFound?.Invoke(file) == false)
                    continue; // event was cancelled, they do not wish to use this file. skip to the next.

                CreateScriptFromFile(file);
            }
        }

        public bool WatchForChanges()
        {
            try
            {
                _watcher = new FileSystemWatcher(ScriptFolder);
                _watcher.Created += _watcher_Created;
                _watcher.Changed += _watcher_Changed;
                _watcher.Deleted += _watcher_Deleted;
                _watcher.Renamed += _watcher_Renamed;
                _watcher.Error += _watcher_Error;
                _watcher.EnableRaisingEvents = true;

                return true;
            }
            catch (Exception ex)
            {
                var orig = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[LUA] FILE WATCHERS ARE NOT RUNNING");
                Console.WriteLine("[LUA] Try running: export MONO_MANAGED_WATCHER=dummy");
                Console.ForegroundColor = orig;
            }
            return false;
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("[LUA] Error");
            Console.WriteLine(e.GetException());
        }

        private void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!Path.GetExtension(e.FullPath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase)) return;
            Console.WriteLine("[LUA] Renamed: " + e.FullPath);
            var src = Path.GetFileNameWithoutExtension(e.OldFullPath);
            var dst = Path.GetFileNameWithoutExtension(e.FullPath);

            foreach (var s in _scripts)
            {
                if (s.FileName.Equals(src))
                {
                    s.FileName = dst;
                    s.FilePath = e.FullPath;
                }
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!Path.GetExtension(e.FullPath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase)) return;

            var name = Path.GetFileNameWithoutExtension(e.FullPath);
            var matches = _scripts.Where(x => x.FileName == name).ToArray();
            Console.WriteLine($"[LUA] Deleted: {e.FullPath} [m:{matches.Count()}]");
            foreach (var script in matches)
            {
                try
                {
                    _scripts.Remove(script);
                    script.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Lua] Unload failed {ex}");
                }
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!Path.GetExtension(e.FullPath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase)) return;
            var name = Path.GetFileNameWithoutExtension(e.FullPath);
            var matches = _scripts.Where(x => x.FileName == name).ToArray();
            Console.WriteLine($"[LUA] Changed: {e.FullPath} [m:{matches.Count()}]");
            foreach (var script in matches)
            {
                script.Load();
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!Path.GetExtension(e.FullPath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase)) return;
            Console.WriteLine("[LUA] Created: " + e.FullPath);
            CreateScriptFromFile(e.FullPath);
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

        public void Cli()
        {
            var exit = false;
            do
            {
                Console.WriteLine("[LUA] TEST MENU. Press C to exit");
                exit = (Console.ReadKey(true).Key == ConsoleKey.C);
            } while (!exit);
        }
    }
}
