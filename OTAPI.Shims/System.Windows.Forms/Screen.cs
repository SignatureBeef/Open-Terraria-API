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
using System.Drawing;

namespace System.Windows.Forms
{
	public class Screen
	{
		public static Screen[] AllScreens { get; }
		public int BitsPerPixel => 0;

		public Rectangle Bounds => default(Rectangle);

		public string DeviceName => "";

		public bool Primary => false;

		public static Screen PrimaryScreen => null;
		public Rectangle WorkingArea { get; }
		public override bool Equals(object obj) => false;
		public static Screen FromPoint(Point point) => null;
		public static Screen FromRectangle(Rectangle rect) => null;
		public static Screen FromControl(Control control) => null;
		public static Screen FromHandle(IntPtr hwnd) => null;
		public static Rectangle GetWorkingArea(Point pt) => default(Rectangle);
		public static Rectangle GetWorkingArea(Rectangle rect) => default(Rectangle);
		public static Rectangle GetWorkingArea(Control ctl) => default(Rectangle);
		public static Rectangle GetBounds(Point pt) => default(Rectangle);
		public static Rectangle GetBounds(Rectangle rect) => default(Rectangle);
		public static Rectangle GetBounds(Control ctl) => default(Rectangle);
		public override int GetHashCode() => 0;

		public override string ToString() => "";
	}
}
