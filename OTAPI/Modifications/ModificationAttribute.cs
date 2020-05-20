using System;
using System.Collections.Generic;
using System.Text;

namespace OTAPI.Modifications
{
    public class ModificationAttribute : Attribute
    {
        public string Description { get; set; }

        public ModificationAttribute(string description)
        {
            this.Description = description;
        }
    }
}
