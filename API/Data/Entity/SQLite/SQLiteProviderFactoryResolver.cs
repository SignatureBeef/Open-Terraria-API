using System;
using System.Data.Entity.Infrastructure;
using System.Data.Common;

namespace OTA.Data.Entity.SQLite
{
    public class SQLiteProviderFactoryResolver : IDbProviderFactoryResolver
    {
        public DbProviderFactory ResolveProviderFactory(DbConnection connection)
        {
            return DbProviderFactories.GetFactory(connection);
        }
    }
}

