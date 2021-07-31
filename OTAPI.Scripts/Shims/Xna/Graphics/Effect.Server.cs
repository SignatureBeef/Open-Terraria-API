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
    public class Effect
    {
        public EffectTechnique CurrentTechnique { get; set; }

        public EffectParameterCollection Parameters { get; set; }
    }

    public class EffectParameterCollection
    {
        public EffectParameter this[string name]
        {
            get
            {
                return null;
            }
            set { }
        }
    }

    public class EffectParameter
    {
        public void SetValue(Texture value) { }

        public void SetValue(string value) { }

        public void SetValue(Matrix[] value) { }

        public void SetValue(Matrix value) { }

        public void SetValue(Quaternion[] value) { }

        public void SetValue(Quaternion value) { }

        public void SetValue(Vector4[] value) { }

        public void SetValue(Vector4 value) { }

        public void SetValue(Vector3[] value) { }

        public void SetValue(Vector3 value) { }

        public void SetValue(Vector2[] value) { }

        public void SetValue(Vector2 value) { }

        public void SetValue(float[] value) { }

        public void SetValue(float value) { }

        public void SetValue(int[] value) { }

        public void SetValue(int value) { }

        public void SetValue(bool[] value) { }

        public void SetValue(bool value) { }

        public void SetValueTranspose(Matrix[] value) { }

        public void SetValueTranspose(Matrix value) { }
    }
}