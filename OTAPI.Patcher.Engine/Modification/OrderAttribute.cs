using System;

namespace OTAPI.Patcher.Engine.Modification
{
    public class OrderedAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderedAttribute(int order = 5)
        {
            this.Order = order;
        }
    }
}
