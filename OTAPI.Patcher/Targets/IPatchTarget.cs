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

using ModFramework;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public interface IPatchTarget
    {
        string DisplayText { get; }
        void Patch();
    }

    public static partial class Common
    {
        public static void AddPatchMetadata(this IPatchTarget target, ModFwModder modder,
            string initialModuleName = null,
            string inputName = null)
        {
            modder.AddMetadata("OTAPI.Target", target.DisplayText);
            if (initialModuleName != null) modder.AddMetadata("OTAPI.ModuleName", initialModuleName);
            if (inputName != null) modder.AddMetadata("OTAPI.Input", inputName);
        }

        public static void WriteCIArtifacts(this IPatchTarget target, string outputFolder)
        {
            if (Directory.Exists(outputFolder)) Directory.Delete(outputFolder, true);
            Directory.CreateDirectory(outputFolder);

            File.Copy("../../../../COPYING.txt", Path.Combine(outputFolder, "COPYING.txt"));
            File.Copy("OTAPI.dll", Path.Combine(outputFolder, "OTAPI.dll"));
            File.Copy("OTAPI.Runtime.dll", Path.Combine(outputFolder, "OTAPI.Runtime.dll"));
        }

        public static void AddEnvMetadata(this IPatchTarget target, ModFwModder modder)
        {
            var commitSha = Common.GetGitCommitSha();
            var run = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")?.Trim();

            if (!String.IsNullOrWhiteSpace(commitSha))
                modder.AddMetadata("GitHub.Commit", commitSha);

            if (!String.IsNullOrWhiteSpace(run))
                modder.AddMetadata("GitHub.Action.RunNo", run);
        }

        public static void AddMarkdownFormatter(this IPatchTarget target)
        {
            const string RepoBase = "https://github.com/DeathCradle/Open-Terraria-API/tree/upcoming";
            const string Scripts = "/OTAPI.Scripts";
            const string HeaderFormat = "| Type | Mod | Platforms | Comment |\n| ---- | ---- | ---- | ---- |";
            MarkdownDocumentor.RegisterTypeFormatter<BasicComment>((header, data) =>
            {
                if (header) return HeaderFormat;

                var filename = Path.GetFileName(data.FilePath);
                var client = data.FilePath.Contains("patchtime", StringComparison.CurrentCultureIgnoreCase);

                var basePath = "";
                if (data.Type == "toplevel")
                    basePath = RepoBase + Scripts + "/TopLevelScripts";
                else if (data.Type == "patch")
                    basePath = RepoBase + Scripts + "/Patches";
                else if (data.Type == "module")
                {
                    basePath = RepoBase + Scripts + "/Shims";
                    filename = Path.GetFileName(Path.GetDirectoryName(data.FilePath));
                }
                else if (data.Type == "script"
                    && Path.GetExtension(data.FilePath).Equals(".lua", StringComparison.CurrentCultureIgnoreCase))
                {
                    basePath = RepoBase + Scripts + "/Lua";
                    filename = String.Join("/",
                        data.FilePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Where(dir => !dir.Equals("lua", StringComparison.CurrentCultureIgnoreCase))
                        .ToArray()
                    );
                }
                else if (data.Type == "script"
                    && Path.GetExtension(data.FilePath).Equals(".js", StringComparison.CurrentCultureIgnoreCase))
                {
                    basePath = RepoBase + Scripts + "/JavaScript";
                    filename = String.Join("/",
                        data.FilePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Where(dir => !dir.Equals("clearscript", StringComparison.CurrentCultureIgnoreCase))
                        .ToArray()
                    );
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

        }
    }
}
