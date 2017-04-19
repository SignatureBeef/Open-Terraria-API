using System;
using System.IO;

namespace Microsoft.Xna.Framework.Content
{
    public sealed class ContentReader : BinaryReader
    {
        public ContentManager ContentManager
        {
            get
            {
                return null; ;
            }
        }

        public string AssetName
        {
            get
            {
                return null;
            }
        }
        
        private ContentReader(ContentManager contentManager, Stream input, string assetName, Action<IDisposable> recordDisposableObject, int graphicsProfile) : base(input)
        {
        }

        public T ReadObject<T>()
        {
            return default(T);
        }

        public T ReadObject<T>(T existingInstance)
        {
            return default(T);
        }

        public T ReadObject<T>(ContentTypeReader typeReader)
        {
            return default(T);
        }

        public T ReadObject<T>(ContentTypeReader typeReader, T existingInstance)
        {
            return default(T);
        }

        public T ReadRawObject<T>()
        {
            return default(T);
        }

        public T ReadRawObject<T>(T existingInstance)
        {
            return default(T);
        }

        public T ReadRawObject<T>(ContentTypeReader typeReader)
        {
            return default(T);
        }

        public T ReadRawObject<T>(ContentTypeReader typeReader, T existingInstance)
        {
            return default(T);
        }

        public void ReadSharedResource<T>(Action<T> fixup)
        {

        }

        public T ReadExternalReference<T>()
        {
            return default(T);
        }

        public Vector2 ReadVector2()
        {
            Vector2 result;
            result.X = this.ReadSingle();
            result.Y = this.ReadSingle();
            return result;
        }

        public Vector3 ReadVector3()
        {
            Vector3 result;
            result.X = this.ReadSingle();
            result.Y = this.ReadSingle();
            result.Z = this.ReadSingle();
            return result;
        }

        public Vector4 ReadVector4()
        {
            Vector4 result;
            result.X = this.ReadSingle();
            result.Y = this.ReadSingle();
            result.Z = this.ReadSingle();
            result.W = this.ReadSingle();
            return result;
        }

        public Matrix ReadMatrix()
        {
            Matrix result;
            result.M11 = this.ReadSingle();
            result.M12 = this.ReadSingle();
            result.M13 = this.ReadSingle();
            result.M14 = this.ReadSingle();
            result.M21 = this.ReadSingle();
            result.M22 = this.ReadSingle();
            result.M23 = this.ReadSingle();
            result.M24 = this.ReadSingle();
            result.M31 = this.ReadSingle();
            result.M32 = this.ReadSingle();
            result.M33 = this.ReadSingle();
            result.M34 = this.ReadSingle();
            result.M41 = this.ReadSingle();
            result.M42 = this.ReadSingle();
            result.M43 = this.ReadSingle();
            result.M44 = this.ReadSingle();
            return result;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion result;
            result.X = this.ReadSingle();
            result.Y = this.ReadSingle();
            result.Z = this.ReadSingle();
            result.W = this.ReadSingle();
            return result;
        }

        public Color ReadColor()
        {
            return new Color
            {
                PackedValue = this.ReadUInt32()
            };
        }

        public unsafe override float ReadSingle()
        {
            return 0;
        }

        public unsafe override double ReadDouble()
        {
            return 0;
        }
    }
}
