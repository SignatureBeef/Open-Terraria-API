using System.Data;

namespace OTA.Data
{
    /// <summary>
    /// Defines an entry point to a database implementation's initialiser.
    /// It is not really designed to handle multiple providers at once,
    /// rather it's because we have multiple provider implementations
    /// in OTAPI and trying to manage all the entry points are a pain.
    /// </summary>
    public interface IDatabaseProvider
    {
        void Initialise(string provider, string connectionString);
        void Migrate();
        IDbConnection CreateConnection();

        string Provider { get; }
        bool Available { get; }
    }
}
