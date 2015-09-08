using System;
using System.Data.Entity.Infrastructure;
using System.Data.Common;

namespace OTA.Data.Entity
{
    class OTAConnectionFactory : IDbConnectionFactory
    {
        public static OTAConnectionFactory Instance = new OTAConnectionFactory();

        DbConnection IDbConnectionFactory.CreateConnection(string nameOrConnectionString)
        {
            var conn = EFConfiguration.ProviderFactory.CreateConnection();
            conn.ConnectionString = nameOrConnectionString;
            return conn;
        }
    }
}

