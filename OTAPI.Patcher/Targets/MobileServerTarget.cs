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

namespace OTAPI.Patcher.Targets;

[MonoMod.MonoModIgnore]
public class MobileServerTarget : PCServerTarget
{
    public override string DisplayText { get; } = "OTAPI Mobile Server";
    public override string ArtifactName { get; } = "artifact-mobile";
    public override IFileResolver FileResolver { get; } = new MobileFileResolver();

    public override NugetPackageBuilder NugetPackager { get; } = new("OTAPI.Mobile.nupkg", "../../../../docs/OTAPI.Mobile.nuspec");
    public override MarkdownDocumentor MarkdownDocumentor { get; } = new("OTAPI.Mobile.Server.mfw.md");
}

