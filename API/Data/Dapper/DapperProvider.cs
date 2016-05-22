using Dapper.Contrib.Extensions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.AssemblyLoader;
using FluentMigrator.Runner.Processors;
using OTA.Data.Dapper.Extensions;
using OTA.Data.Dapper.Mappers;
using OTA.Extensions;
using OTA.Logging;
using System.Data;
using System.IO;
using System.Linq;

namespace OTA.Data.Dapper
{
    public class DapperProvider : IDatabaseProvider
    {
        private string _provider, _connectionString;
        private System.Reflection.ConstructorInfo _providerConstructor;
        private LogChannel _logger => new LogChannel("Migration", System.ConsoleColor.Yellow, System.Diagnostics.TraceLevel.Info)
        {
            EnableConsoleOutput = false
        };

        public string Provider { get { return _provider; } }
        public bool Available { get { return _providerConstructor != null; } }

        public void Initialise(string provider, string connectionString)
        {
            _provider = provider;
            _connectionString = connectionString;

            //Default shortcuts.
            var lowered = provider.ToLower();
            if (lowered == "sqlite")
            {
                SetProviderType("SQLiteConnection", "System.Data.SQLite, Version=1.0.99.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
            }
            else if (new[] { "mysql", "mariadb" }.Contains(lowered))
            {
                SetProviderType("MySqlConnection", "MySql.Data, Version=6.9.8.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d");
                _provider = "mysql";
            }
            else if (new[] { "mssql", "sqlserver" }.Contains(lowered))
            {
                SetProviderType<System.Data.SqlClient.SqlConnection>();
                _provider = "sqlserver";
            }
            else if (lowered == "postgres")
            {
                SetProviderType("NpgsqlConnection", "Npgsql, Version=3.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7");
                SqlMapperExtensions.TableNameMapper += (type) =>
                {
                    return TableMapper.TypeToName(type);
                };
            }

            //Enable logging for the migrations announcer
            _logger.Targets.Add(
                 new FileOutputTarget(Path.Combine(Globals.LogFolderPath, "migrations.log"))
             );
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
#else
            var announcer = new FluentMigrator.Runner.Announcers.TextWriterAnnouncer((str) =>
            {
                _logger.Log(str);
            });
#endif
            var ctx = new RunnerContext(announcer)
            {
                Database = _provider,
                Connection = _connectionString,
                Targets = Plugin.PluginManager.Loaded.Names.ToArray()
            };

            new TaskExecutor(ctx, new DapperPluginAssemblyFactory(), new MigrationProcessorFactoryProvider()).Execute();
        }

        //TODO: look into DbProviderFactories
        public void SetProviderType(string typeName, string assemblyName = null)
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

            if (type == null && assemblyName != null)
            {
                type = System.Reflection.Assembly.Load(assemblyName)
                    .GetTypesLoaded()
                    .Where(x => x.Name == typeName)
                    .SingleOrDefault();
            }

            if (type != null)
            {
                _providerConstructor = type.GetConstructor(new System.Type[] { typeof(string) });
            }
        }

        public void SetProviderType<T>()
        {
            _providerConstructor = typeof(T).GetConstructor(new System.Type[] { typeof(string) });
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
