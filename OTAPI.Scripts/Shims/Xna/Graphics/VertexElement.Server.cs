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

namespace Microsoft.Xna.Framework.Graphics
{
    public enum VertexElementFormat
    {
        Single,
        Vector2,
        Vector3,
        Vector4,
        Color,
        Byte4,
        Short2,
        Short4,
        NormalizedShort2,
        NormalizedShort4,
        HalfVector2,
        HalfVector4
    }
    public enum VertexElementUsage
    {
        Position,
        Color,
        TextureCoordinate,
        Normal,
        Binormal,
        Tangent,
        BlendIndices,
        BlendWeight,
        Depth,
        Fog,
        PointSize,
        Sample,
        TessellateFactor
    }

    public struct VertexElement
    {

        public int Offset { get; set; }

        public VertexElementFormat VertexElementFormat { get; set; }

        public VertexElementUsage VertexElementUsage { get; set; }

        public int UsageIndex { get; set; }

        public VertexElement(int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage, int usageIndex)
        {
            this.Offset = offset;
            this.VertexElementFormat = elementFormat;
            this.VertexElementUsage = elementUsage;
            this.UsageIndex = usageIndex;
        }

        public override int GetHashCode() => 0;

        public override string ToString() => "";

        public override bool Equals(object obj) => false;

        public static bool operator ==(VertexElement left, VertexElement right) => false;

        public static bool operator !=(VertexElement left, VertexElement right) => false;
    }
}