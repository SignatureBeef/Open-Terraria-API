namespace Microsoft.Xna.Framework.Content
{
    public abstract class ContentTypeReader<T> : ContentTypeReader
    {
        protected ContentTypeReader() : base(typeof(T))
        {
        }

        protected internal override object Read(ContentReader input, object existingInstance)
        {
            return null;
        }

        protected internal abstract T Read(ContentReader input, T existingInstance);
    }
}
