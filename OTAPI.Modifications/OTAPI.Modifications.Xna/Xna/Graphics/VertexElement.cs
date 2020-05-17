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