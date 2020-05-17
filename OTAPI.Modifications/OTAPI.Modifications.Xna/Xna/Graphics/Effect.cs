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