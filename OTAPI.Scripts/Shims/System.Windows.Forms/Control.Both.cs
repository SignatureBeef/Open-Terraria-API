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
	public class Control
	{
		public Drawing.Point Location { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public int Top { get; set; }
		public int Left { get; set; }
		public Padding Margin { get; set; }
		public Rectangle Bounds { get; set; }

		public static Control FromHandle(IntPtr handle) => null;

		public Size MinimumSize { get; set; }
		public Size MaximumSize { get; set; }

		public void SendToBack() { }
		public void BringToFront() { }
	}

	[Serializable]
	public struct Padding
	{

		/// <summary>Provides a <see cref="T:System.Windows.Forms.Padding" /> object with no padding.</summary>
		/// <filterpriority>1</filterpriority>
		public static readonly Padding Empty = new Padding(0);

		public int All { get; set; }
		public int Bottom { get; set; }
		public int Left { get; set; }
		public int Right { get; set; }

		public int Top { get; set; }

		public int Horizontal => Left + Right;

		public int Vertical => Top + Bottom;

		public Size Size => new Size(Horizontal, Vertical);

		public Padding(int all)
		{
			All = Bottom = Left = Right = Top = 0;
		}

		public Padding(int left, int top, int right, int bottom)
		{
			All = Bottom = Left = Right = Top = 0;
		} 

		public static Padding Add(Padding p1, Padding p2) => default(Padding);
		public static Padding Subtract(Padding p1, Padding p2) => default(Padding);
		public override bool Equals(object other) => false;
		public static Padding operator +(Padding p1, Padding p2) => default(Padding);
		public static Padding operator -(Padding p1, Padding p2) => default(Padding);
		public static bool operator ==(Padding p1, Padding p2) => false; 
		public static bool operator !=(Padding p1, Padding p2) => false;
		public override int GetHashCode() => 0;

		public override string ToString() => "";
	}
}
