using System;

namespace OTA.Patcher
{
    /// <summary>
    /// Used to mark a method as an OTA patcher method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OTAPatchAttribute : Attribute
    {
        /// <summary>
        /// The text to be displayed in the console when the method is being executed.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// What modes the patch method supports.
        /// </summary>
        public SupportType SupportedTypes { get; set; }

        /// <summary>
        /// This allows you to lead or lag the call of the patch method.
        /// </summary>
        public int Order { get; set; }

        public OTAPatchAttribute(SupportType supportedTypes, string text, int order = 100)
        {
            this.SupportedTypes = supportedTypes;
            this.Text = text;
            this.Order = order;
        }
    }
}

