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

namespace Microsoft.Xna.Framework.Graphics
{
    public class DisplayMode
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }

    public enum BufferUsage
    {
        WriteOnly,
        None
    }

    public class DynamicVertexBuffer : VertexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;

        public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice,
            VertexDeclaration vertexDeclaration,
            int vertexCount,
            BufferUsage usage
        )
        {
        }

        public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice,
            Type vertexType,
            int vertexCount,
            BufferUsage usage
        )
        {
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride, SetDataOptions options) where T : struct
        {

        }

        public void SetData<T>(T value)
        {
        }
    }

    public enum IndexElementSize
    {
        ThirtyTwoBits,
        SixteenBits
    }

    public class DynamicIndexBuffer : IndexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;

        public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice,
            IndexElementSize indexElementSize,
            int indexCount,
            BufferUsage usage
        )
        {
        }

        public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice,
            Type indexType,
            int indexCount,
            BufferUsage usage
        )
        {
        }
    }

    public interface IVertexType
    {

    }

    public class VertexDeclaration : GraphicsResource
    {
        public int VertexStride { get; }

        public VertexDeclaration(int vertexStride, params VertexElement[] elements) { }

        public VertexDeclaration(params VertexElement[] elements) { }

        public VertexElement[] GetVertexElements() => null;
    }

    public struct VertexPositionColorTexture : IVertexType
    {
        public Color Color;
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public static readonly VertexDeclaration VertexDeclaration;
    }

    public class IndexBuffer : GraphicsResource
    {
        public void SetData<T>(
            int offsetInBytes,
            T[] data,
            int startIndex,
            int elementCount
        )
        {
        }

        public void SetData<T>(
            T[] data
        )
        {
        }

        public void SetData<T>(
            T[] data,
            int startIndex,
            int elementCount
        )
        {

        }
    }
}