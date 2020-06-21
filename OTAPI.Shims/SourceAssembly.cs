using System;

namespace OTAPI
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class SourceAssemblyAttribute : Attribute
    {
        public string FileName { get; set; }
        public string ModuleName { get; set; }
    }
}