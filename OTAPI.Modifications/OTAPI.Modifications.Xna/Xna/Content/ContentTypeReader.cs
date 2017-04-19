using System;

namespace Microsoft.Xna.Framework.Content
{
    public abstract class ContentTypeReader
    {
        private Type targetType;

        internal readonly bool TargetIsValueType;

        public Type TargetType
        {
            get
            {
                return this.targetType;
            }
        }

        public virtual int TypeVersion
        {
            get
            {
                return 0;
            }
        }

        public virtual bool CanDeserializeIntoExistingObject
        {
            get
            {
                return false;
            }
        }

        protected ContentTypeReader(Type targetType)
        {
            this.targetType = targetType;
            if (targetType != null)
            {
                this.TargetIsValueType = targetType.IsValueType;
            }
        }

        protected internal virtual void Initialize(ContentTypeReaderManager manager)
        {
        }

        protected internal abstract object Read(ContentReader input, object existingInstance);
    }
}
