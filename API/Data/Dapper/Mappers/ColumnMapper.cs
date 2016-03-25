using System;

namespace OTA.Data.Dapper.Mappers
{
    public static class ColumnMapper
    {
        public static string Enclose(string columnName)
        {
            if(DatabaseFactory.Provider == "postgres")
            {
                return '"' + columnName + '"';
            }
            return columnName;
        }
    }
}
