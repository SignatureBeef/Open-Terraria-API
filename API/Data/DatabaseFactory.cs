
using System.Data;

namespace OTA.Data
{
    public static class DatabaseFactory
    {
        private static readonly IDatabaseProvider _provider = new Dapper.DapperProvider();

        public static void Initialise(string provider, string connectionString) => _provider.Initialise(provider, connectionString);

        public static IDbConnection CreateConnection() => _provider.CreateConnection();
    }
}
