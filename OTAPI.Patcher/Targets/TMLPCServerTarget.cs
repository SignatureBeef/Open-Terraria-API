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
using OTAPI.Patcher.Resolvers;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Targets;

[MonoMod.MonoModIgnore]
public class TMLPCServerTarget : PCServerTarget
{
    public override string DisplayText { get; } = "TML PC Server";
    public override string ArtifactName { get; } = "artifact-tml";
    public override IFileResolver FileResolver { get; } = new TMLFileResolver();

    public override NugetPackageBuilder NugetPackager { get; } = new("OTAPI.TML.nupkg", "../../../../docs/OTAPI.TML.nuspec");
    public override MarkdownDocumentor MarkdownDocumentor { get; } = new("OTAPI.TML.PC.Server.mfw.md");

    public override bool PublicEverything => false; // tml expects various classes to still be private

    public override void AddSearchDirectories(ModFwModder modder)
    {
        base.AddSearchDirectories(modder);

        var folders = Directory.GetFiles(Path.Combine("tModLoader", "Libraries"), "*.dll", SearchOption.AllDirectories)
                .Select(x => Path.GetDirectoryName(x))
                .Distinct();

        foreach (var folder in folders)
            modder.AssemblyResolver.AddSearchDirectory(folder);
    }

    public override void MergeReLogic(ModFwModder modder, string embeddedResources) { } // tml has this in libraries

    public override void AddVersion(ModFwModder modder) { } // dont break tml versions

    public override void CompileAndReadShims(ModFwModder modder) { } // tml doesn need shims

    public override void LoadModifications()
    {
        base.LoadModifications();

        ModContext.ReferenceFiles.Add(Path.Combine("tModLoader", "Libraries", "FNA", "1.0.0", "FNA.dll"));
    }
}
