
using System.Data;

namespace OTA.Data
{
    public static class DatabaseFactory
    {
        private static readonly IDatabaseProvider _provider = new Dapper.DapperProvider();

        public static void Initialise(string provider, string connectionString) => _provider.Initialise(provider, connectionString);
        public static void Migrate() => _provider.Migrate();

        public static string Provider { get { return _provider.Provider; } }

        public static IDbConnection CreateConnection()
        {
            var conn = _provider.CreateConnection();
            conn.Open();
            return conn;
        }
    }
}
