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
#pragma warning disable CS0436 // Type conflicts with imported type

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
	public abstract class Texture : GraphicsResource
	{

	}

	public class Texture2D : Texture
	{
		public static Texture2D[] Array;

		public int Height { get; set; }

		public int Width { get; set; }

		public Texture2D (
			GraphicsDevice graphicsDevice,
			int width,
			int height,
			bool mipMap,
			SurfaceFormat format
		)
		{
		}

		public Texture2D (
			GraphicsDevice graphicsDevice,
			int width,
			int height
		)
		{
		}

		public void SetData<T> (T[] data, int startIndex, int elementCount) where T : struct
		{
			//this.SetData<T>(0, null, data, startIndex, elementCount);
		}
		public void SetData<T>(T[] data) where T : struct
		{

		}
		public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
		{
		}

		public void GetData<T> (
			T[] data
		)
		{

		}

		public void GetData<T> (
			T[] data,
			int startIndex,
			int elementCount
		)
		{

		}

		public void GetData<T> (
			int level,
			Nullable<Rectangle> rect,
			T[] data,
			int startIndex,
			int elementCount
		)
		{

		}

        public static Texture2D FromStream(GraphicsDevice graphicsDevice, System.IO.Stream stream) => null;
        public static Texture2D FromStream(GraphicsDevice graphicsDevice, System.IO.Stream stream, int width, int height, [MarshalAs(UnmanagedType.U1)] bool zoom) => null;
    }
}