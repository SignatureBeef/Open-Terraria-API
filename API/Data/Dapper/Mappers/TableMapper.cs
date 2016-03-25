using Dapper.Contrib.Extensions;
using System;

namespace OTA.Data.Dapper.Mappers
{
    public static class TableMapper
    {
        public static string TypeToName<T>()
        {
            return TypeToName(typeof(T));
        }

        public static string TypeToName(Type type)
        {
            var r = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            if (r != null)
            {
                return r.Name;
            }
            return type.Name + 's';
        }
    }
}
