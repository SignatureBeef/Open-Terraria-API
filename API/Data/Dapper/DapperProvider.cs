using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.AssemblyLoader;
using FluentMigrator.Runner.Processors;
using OTA.Extensions;
using System.Data;
using System.Linq;

namespace OTA.Data.Dapper
{
    public class DapperProvider : IDatabaseProvider
    {
        private string _provider, _connectionString;
        private System.Reflection.ConstructorInfo _providerConstructor;

        public void Initialise(string provider, string connectionString)
        {
            System.IO.File.Delete("database.sqlite");
            _provider = provider;
            _connectionString = connectionString;

            //Default shortcuts.
            if (provider.ToLower() == "sqlite")
            {
                SetProviderType("SQLiteConnection");
            }
        }

        public void Migrate()
        {
            // To compliment our Dapper implementation we use the Fluent Migrator (+Runner) to solve various database
            // update problems that can arise due to numerous updates to OTAPI itself and it's plugins.

            // Construct a RunnerContext from the Fluent Migrator Runner assembly and pass it our plugin assemblies,
            // and OTAPI itself (todo when we use migrations) for the runner to scan and apply migrations automatically.
            // For debugging we should use the default console announcer, but for release mode we should really write
            // to a migration log e.g. Data\logs\migration_yyyyMMddHHmmss.log
#if DEBUG
            var announcer = new FluentMigrator.Runner.Announcers.ConsoleAnnouncer();
#endif
            var ctx = new RunnerContext(announcer)
            {
                Database = _provider,
                Connection = _connectionString,
                Targets = Plugin.PluginManager.LoadedPlugins.ToArray()
            };

            new TaskExecutor(ctx, new DapperPluginAssemblyFactory(), new MigrationProcessorFactoryProvider()).Execute();
        }

        public void SetProviderType(string typeName)
        {
            var type = System.AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypesLoaded())
                .Where(x => x.FullName == typeName)
                .SingleOrDefault();

            if (type == null)
            {
                type = System.AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypesLoaded())
                    .Where(x => x.Name == typeName)
                    .SingleOrDefault();
            }

            if (type != null)
            {
                _providerConstructor = type.GetConstructor(new System.Type[] { typeof(string) });
            }
        }

        public IDbConnection CreateConnection()
        {
            return (IDbConnection)_providerConstructor.Invoke(new object[] { _connectionString });
        }
    }

    public class DapperPluginAssemblyResolver : IAssemblyLoader
    {
        private readonly string name;

        public DapperPluginAssemblyResolver(string name)
        {
            this.name = name;
        }

        public System.Reflection.Assembly Load()
        {
            return Plugin.PluginManager.GetPlugin(name).Assembly;
        }
    }

    public class DapperPluginAssemblyFactory : AssemblyLoaderFactory
    {
        public override IAssemblyLoader GetAssemblyLoader(string name)
        {
            var nameInLowerCase = name.ToLower();

            if (nameInLowerCase.EndsWith(".dll") || nameInLowerCase.EndsWith(".exe"))
                return new AssemblyLoaderFromFile(name);

            return new DapperPluginAssemblyResolver(name);
        }
    }
}
