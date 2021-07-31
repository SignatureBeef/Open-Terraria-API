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
