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
using System.Runtime.InteropServices;

namespace OTAPI.Common
{
    public class InstallStatusUpdate : EventArgs
    {
        public string Text { get; set; }
    }

    public abstract class BaseInstallDiscoverer : IInstallDiscoverer
    {
        public event EventHandler<InstallStatusUpdate> StatusUpdate;

        public abstract string[] SearchPaths { get; }

        public abstract bool IsValidInstallPath(string folder);

        public abstract OSPlatform GetClientPlatform();

        public abstract string GetResource(string fileName, string installPath);
        public abstract string GetResourcePath(string installPath);

        public virtual IEnumerable<string> FindInstalls()
        {
            foreach (var path in SearchPaths)
            {
                var formatted = path.Replace("[USER_NAME]", Environment.UserName);
                if (IsValidInstallPath(formatted))
                    yield return formatted;
            }
        }

        public string Status
        {
            set => StatusUpdate?.Invoke(this, new InstallStatusUpdate() { Text = value });
        }
    }
}
