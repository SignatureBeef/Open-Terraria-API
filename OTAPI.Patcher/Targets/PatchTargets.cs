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
using ModFramework;
using ModFramework.Modules.CSharp;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public static class PatchTargets
    {
        public static void Log(this IPatchTarget target, string message) => Common.Log(message);
        public static string GetCliValue(this IPatchTarget target, string key) => Common.GetCliValue(key);

        static Dictionary<char, IPatchTarget> _targets = new Dictionary<char, IPatchTarget>()
        {
            {'p', new OTAPIPCServerTarget() },
            {'m', new OTAPIMobileServerTarget() },
            {'c', new OTAPIClientLightweightTarget() },
            {'t', new TMLPCServerTarget() },
        };

        public static IPatchTarget DeterminePatchTarget()
        {
            const string RepoBase = "https://github.com/DeathCradle/Open-Terraria-API/tree/upcoming/OTAPI.Scripts";
            const string HeaderFormat = $"| Type | Mod | Platforms | Comment |\n| ---- | ---- | ---- | ---- |";
            MarkdownDocumentor.RegisterTypeFormatter<BasicComment>((header, data) =>
            {
                if (header) return HeaderFormat;

                var filename = Path.GetFileName(data.FilePath);

                var basePath = "";
                if (data.Type == "toplevel")
                    basePath = RepoBase + "/TopLevelScripts";
                else if (data.Type == "patch")
                    basePath = RepoBase + "/Patches";
                else if (data.Type == "module")
                {
                    basePath = RepoBase + "/Shims";
                    filename = Path.GetFileName(Path.GetDirectoryName(data.FilePath));
                }
                else if (data.Type == "script"
                    && Path.GetExtension(data.FilePath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase))
                {
                    basePath = RepoBase + "/Lua";
                    filename = data.FilePath.Replace("lua/", "");
                }
                else if (data.Type == "script"
                    && Path.GetExtension(data.FilePath).Equals(".js", StringComparison.CurrentCultureIgnoreCase))
                {
                    basePath = RepoBase + "/JavaScript";
                    filename = data.FilePath.Replace("clearscript/", "");
                }

                var platforms = "";
                if (data.FilePath.Contains(".Both", StringComparison.CurrentCultureIgnoreCase)
                    || data.FilePath.Contains("-Both", StringComparison.CurrentCultureIgnoreCase))
                    platforms = "Client & Server";
                else if (data.FilePath.Contains(".Server", StringComparison.CurrentCultureIgnoreCase)
                    || data.FilePath.Contains("-Server", StringComparison.CurrentCultureIgnoreCase))
                    platforms = "Server";
                else if (data.FilePath.Contains(".Client", StringComparison.CurrentCultureIgnoreCase)
                    || data.FilePath.Contains("-Client", StringComparison.CurrentCultureIgnoreCase))
                    platforms = "Client";

                return $"| {data.Type} | [{filename}]({basePath}/{filename}) | {platforms} | {data.Comments} |";
            });

            var cli = Common.GetCliValue("patchTarget");

            if (!String.IsNullOrWhiteSpace(cli) && _targets.TryGetValue(cli[0], out IPatchTarget match))
                return match;

            int attempts = 5;
            do
            {
                Console.Write("Which target would you like?\n");

                foreach (var item in _targets.Keys)
                    Console.Write($"\t {item} - {_targets[item].DisplayText}\n");

                Console.Write(": ");

                var input = Console.ReadKey(true);

                Console.WriteLine(input.Key);

                if (_targets.TryGetValue(input.KeyChar.ToString().ToLower()[0], out IPatchTarget inputMatch))
                    return inputMatch;

                if (input.Key == ConsoleKey.Enter) // no key entered
                    break;
            } while (attempts-- > 0);

            return new OTAPIPCServerTarget();
        }
    }
}
