using Dapper.Contrib.Extensions;
using System;

namespace OTA.Data.Dapper.Mappers
{
    public static class TableMapper
    {
        public static string TypeToName<T>(bool enclose = true)
        {
            return TypeToName(typeof(T), enclose);
        }

        public static string TypeToName(Type type, bool enclose = true)
        {
            var r = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            var tableName = String.Empty;
            if (r != null)
            {
                tableName = r.Name;
            }
            else tableName = type.Name + 's';

            if (enclose && DatabaseFactory.Provider == "postgres")
            {
                return '"' + tableName + '"';
            }
            return tableName;
        }
    }
}
