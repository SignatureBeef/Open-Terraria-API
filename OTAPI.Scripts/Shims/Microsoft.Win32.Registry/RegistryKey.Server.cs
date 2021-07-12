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

namespace Microsoft.Win32
{
    public class Registry
    {
        public static RegistryKey CurrentUser;
    }

    public class RegistryKey : IDisposable
    {
        public RegistryKey CreateSubKey(string _) => null;
        public RegistryKey OpenSubKey(string _) => new RegistryKey();

        public object GetValue(string _) => null;
        public static RegistryKey OpenBaseKey(RegistryHive hive, RegistryView view) => new RegistryKey();

        public void SetValue(string name, string value) { } // only seen this in tml (Method not found)

        public void Dispose() { }
    }

    public enum RegistryHive
    {
        ClassesRoot = -2147483648,
        CurrentConfig = -2147483643,
        CurrentUser = -2147483647,
        LocalMachine = -2147483646,
        PerformanceData = -2147483644,
        Users = -2147483645
    }

    public enum RegistryView
    {
        Default = 0,
        Registry32 = 512,
        Registry64 = 256
    }
}
