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
namespace System.Windows.Forms
{
    public struct Message
    {
        public IntPtr HWnd { get; set; }
        public int Msg { get; set; }
        public IntPtr WParam { get; set; }
        public IntPtr LParam { get; set; }
        public IntPtr Result { get; set; }
        public object GetLParam(Type cls) => null;
        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => default(Message);
        public override bool Equals(object o) => false;
        public static bool operator !=(Message a, Message b) => false;
        public static bool operator ==(Message a, Message b) => false;
        public override int GetHashCode() => 0;
        public override string ToString() => "";
    }
}
