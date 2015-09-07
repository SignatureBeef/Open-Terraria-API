using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using OTA.Logging;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Common;
using System.Data;
using System.Data.Entity.Infrastructure.Annotations;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTA.Data.Models
{
    public class PlayerGroup
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }
    }

    public class PermissionNode
    {
        public int Id { get; set; }

        public string Node { get; set; }

        public Permission Permission { get; set; }
    }

    public class PlayerNode
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int NodeId { get; set; }
    }

    public class GroupNode
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int NodeId { get; set; }
    }

    public class APIAccount
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public PasswordFormat PasswordFormat { get; set; }

    }

    public enum PasswordFormat
    {
        SHA256 = 1
    }

    //    class SqliteDbConfiguration : DbConfiguration
    //    {
    //        public SqliteDbConfiguration()
    //        {
    //            string assemblyName = typeof (System.Data.SQLite.EF6.SQLiteProviderFactory).Assembly.GetName().Name;
    //
    //            RegisterDbProviderFactories(assemblyName );
    //            SetProviderFactory(assemblyName, System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);
    //            SetProviderServices(assemblyName,
    //                (DbProviderServices) System.Data.SQLite.EF6.SQLiteProviderFactory.Instance.GetService(
    //                    typeof (DbProviderServices)));
    //        }
    //
    //        static void RegisterDbProviderFactories(string assemblyName)
    //        {
    //            var dataSet = System.Configuration.ConfigurationManager.GetSection("system.data") as DataSet;
    //            if (dataSet != null)
    //            {
    //                var dbProviderFactoriesDataTable = dataSet.Tables.OfType<DataTable>()
    //                    .First(x => x.TableName == typeof (DbProviderFactories).Name);
    //
    //                var dataRow = dbProviderFactoriesDataTable.Rows.OfType<DataRow>()
    //                    .FirstOrDefault(x => x.ItemArray[2].ToString() == assemblyName);
    //
    //                if (dataRow != null)
    //                    dbProviderFactoriesDataTable.Rows.Remove(dataRow);
    //
    //                dbProviderFactoriesDataTable.Rows.Add(
    //                    "SQLite Data Provider (Entity Framework 6)",
    //                    ".NET Framework Data Provider for SQLite (Entity Framework 6)",
    //                    assemblyName,
    //                    typeof (System.Data.SQLite.EF6.SQLiteProviderFactory).AssemblyQualifiedName
    //                );
    //            }
    //        }
    //    }

    /// <summary>
    /// The connection context for talking to an OTA database
    /// </summary>
    //    [DbConfigurationType(typeof(MyDbConfiguration))] 
    public class OTAContext : DbContext // IdentityDbContext<IdentityUser>
    {
        public OTAContext() : base("terraria_ota")//ConnectionManager.ConnectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OTA.Data.Models.OTAContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name or connection string. Default is terraria_ota</param>
        public OTAContext(string nameOrConnectionString = "terraria_ota") : base(nameOrConnectionString)
        {
            
        }

        public DbSet<PlayerGroup> PlayerGroups { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<DbPlayer> Players { get; set; }

        public DbSet<PermissionNode> Nodes { get; set; }

        public DbSet<PlayerNode> PlayerNodes { get; set; }

        public DbSet<GroupNode> GroupNodes { get; set; }

        public DbSet<APIAccount> APIAccounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer(new SqliteContextInitializer<OTAContext>(builder));

            builder.Entity<Group>()
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<DbPlayer>()
                .ToTable("Player")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<PlayerGroup>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<PermissionNode>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<PlayerNode>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<GroupNode>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<APIAccount>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }

    //Based off https://gist.github.com/flaub/1968486e1b3f2b9fddaf#file-sqlitecontextinitializer-cs
    class SqliteContextInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        DbModelBuilder _modelBuilder;

        static readonly Dictionary<String,String> DataTypeMap = new Dictionary<String,String>()
            {
                { "int", "integer" }
            };

        public static string GetDataType(string typeName)
        {
            if (DataTypeMap.ContainsKey(typeName)) return DataTypeMap[typeName];

            return typeName;
        }

        public SqliteContextInitializer(DbModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void InitializeDatabase(T context)
        {
            if (context.Database.Exists())
                return;
            
            var model = _modelBuilder.Build(context.Database.Connection);

            using (var xact = context.Database.BeginTransaction())
            {
                try
                {
                    CreateDatabase(context.Database, model);
                    xact.Commit();
                }
                catch (Exception)
                {
                    xact.Rollback();
                    throw;
                }
            }
        }

        class Index
        {
            public string Name { get; set; }

            public string Table { get; set; }

            public List<string> Columns { get; set; }
        }

        private void CreateDatabase(Database db, DbModel model)
        {
            const string tableTmpl = "CREATE TABLE [{0}] (\n{1}\n);";
            const string columnTmpl = "    [{0}] {1} {2}"; // name, type, decl
            const string primaryKeyTmpl = "    PRIMARY KEY ({0})";
            const string foreignKeyTmpl = "    FOREIGN KEY ({0}) REFERENCES {1} ({2})";
            const string indexTmpl = "CREATE INDEX {0} ON {1} ({2});";

            var indicies = new Dictionary<string, Index>();

            foreach (var type in model.StoreModel.EntityTypes)
            {
                var defs = new List<string>();

                // columns
                foreach (var p in type.Properties)
                {
                    var decls = new HashSet<string>();

                    bool identity = p.StoreGeneratedPattern == System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity;
                    if (identity)
                    {
                        decls.Add("PRIMARY KEY");
                    }
                    else if (!p.Nullable)
                        decls.Add("NOT NULL");

                    var annotations = p.MetadataProperties
                        .Select(x => x.Value)
                        .OfType<IndexAnnotation>();

                    foreach (var annotation in annotations)
                    {
                        
                        foreach (var attr in annotation.Indexes)
                        {
                            if (attr.IsUnique)
                                decls.Add("UNIQUE");

                            if (string.IsNullOrEmpty(attr.Name))
                                continue;

                            Index index;
                            if (!indicies.TryGetValue(attr.Name, out index))
                            {
                                index = new Index
                                {
                                    Name = attr.Name,
                                    Table = type.Name,
                                    Columns = new List<string>(),
                                };
                                indicies.Add(index.Name, index);
                            }
                            index.Columns.Add(p.Name);
                        }
                    }

                    defs.Add(string.Format(columnTmpl, p.Name, GetDataType(p.TypeName), string.Join(" ", decls)));
                }

                // primary keys
                if (type.KeyProperties.Any(x => x.StoreGeneratedPattern != System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity))
                {
                    var keys = type.KeyProperties.Where(x => x.StoreGeneratedPattern != System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity).Select(x => x.Name);
                    defs.Add(string.Format(primaryKeyTmpl, string.Join(", ", keys)));
                }

                // foreign keys
                foreach (var assoc in model.StoreModel.AssociationTypes)
                {
                    if (assoc.Constraint.ToRole.Name == type.Name)
                    {
                        var thisKeys = assoc.Constraint.ToProperties.Select(x => x.Name);
                        var thatKeys = assoc.Constraint.FromProperties.Select(x => x.Name);
                        defs.Add(string.Format(foreignKeyTmpl,
                                string.Join(", ", thisKeys),
                                assoc.Constraint.FromRole.Name,
                                string.Join(", ", thatKeys)));
                    }
                }

                // create table
                var sql = string.Format(tableTmpl, type.Name, string.Join(",\n", defs));
                Console.WriteLine(sql);
                db.ExecuteSqlCommand(sql);
            }

            // create index
            foreach (var index in indicies.Values)
            {
                var columns = string.Join(", ", index.Columns);
                var sql = string.Format(indexTmpl, index.Name, index.Table, columns);
                db.ExecuteSqlCommand(sql);
            }
        }
    }

    /// <summary>
    /// Database connection manager.
    /// </summary>
    public static class ConnectionManager
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// Fetch a new disposable context
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="verbose">If set to <c>true</c> verbose.</param>
        public static OTAContext NewContext
        {
            get
            {
                Console.WriteLine("Selecting: " + (ConnectionManager.ConnectionString ?? "Null"));
                return new OTAContext(ConnectionString);
            }
        }

        /// <summary>
        /// Prepares the database for connection.
        /// </summary>
        /// <remarks>It will load the configuration using the assembly specified.</remarks>
        /// <param name="assemblyName">Assembly name containing the configuration class.</param>
        /// <param name="verbose">If set to <c>true</c> verbose.</param>
        public static bool PrepareFromAssembly(string assemblyName, bool verbose = false)
        {
//            Database.SetInitializer<OTAContext>(new DropCreateDatabaseAlways<OTAContext>());
//            Database.SetInitializer<OTAContext>(new DropCreateDatabaseIfModelChanges<OTAContext>());

//            var asmName = "MySql.Data.Entity";

//            MyDbConfiguration.FactoryName = "System.Data.SQLite.EF6";
////            MyDbConfiguration.FactoryName = "Mono.Data.Sqlite";
//            MyDbConfiguration.FactoryInstance = new System.Data.SQLite.EF6.SQLiteProviderFactory();
//            MyDbConfiguration.FactoryServices = new System.Data.SQLite.EF6.SQLiteProviderFactory();
////            MyDbConfiguration.FactoryInstance = new Mono.Data.Sqlite.SqliteFactory();
////            var r = new  SQLite.Net.Async.SQLiteAsyncConnection();
//
            return true;

            try
            {
                assemblyName = "System.Data.SQLite,";
                //Find the assembly by name that was pre-loaded from the Libraries folder
                var providers = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(x => x.FullName.StartsWith(assemblyName))
                    .ToArray();
//                    .FirstOrDefault(x => x.FullName.StartsWith(assemblyName));

                //Ensure we have at least one provider that matches
                if (providers != null)
                {
                    var provider = providers[0];
                    //Load the configuration from the found provider

//                    DbConfiguration.Loaded += (sender, args) =>
//                    {
//                        Console.WriteLine("Loaded");
//                        args.ReplaceService<IDbConnectionFactory>((s, a) => DbFactory.Instance); 
//                        args.ReplaceService<IDbProviderFactoryResolver>((s, a) => TestProviderFactoryResolver.Instance);
//                    };
//                    
//                    var cf = provider.DefinedTypes.Where(x => typeof(System.Data.Common.DbProviderFactory).IsAssignableFrom(x)).FirstOrDefault();
//
//                    if (cf != null)
//                    {
//                        MyDbConfiguration.FactoryName = "System.Data.SQLite";
//                        MyDbConfiguration.FactoryInstance = (System.Data.Common.DbProviderFactory)Activator.CreateInstance(cf);
//                    }
//                    assemblyName = "System.Data.SQLite.E";
//                    //Find the assembly by name that was pre-loaded from the Libraries folder
//                    provider = AppDomain.CurrentDomain
//                        .GetAssemblies()
//                        .Where(x => x.FullName.StartsWith(assemblyName))
//                        .ToArray()[0];
//                    System.Data.Entity.DbConfiguration.LoadConfiguration(provider);
//                    //Mono.Data.Sqlite
//                    cf = provider.DefinedTypes.Where(x => typeof(DbProviderServices).IsAssignableFrom(x)).FirstOrDefault();
//                    if (cf != null)
//                    {
//                        MyDbConfiguration.FactoryServices = (DbProviderServices)Activator.CreateInstance(cf);
//                    }

//                    Database.SetInitializer(new MigrateDatabaseToLatestVersion<OTAContext, OTADatabaseInitializer>());

//                    var config = new OTADatabaseInitializer();
//                    var scaffolder = new MigrationScaffolder(config);
//                    var migration = scaffolder.Scaffold("AddGroupsTable");
//
//                    File.WriteAllText(migration.MigrationId + ".cs", migration.UserCode);
//                    File.WriteAllText(migration.MigrationId + ".Designer.cs", migration.DesignerCode);

//                    using (var writer = new ResXResourceWriter(migration.MigrationId + ".resx"))
//                    {
//                        foreach (var resource in migration.Resources)
//                        {
//                            writer.AddResource(resource.Key, resource.Value);
//                        }
//                    }


//                    var migrator = new DbMigrator(new OTADatabaseInitializer() );
//                    migrator.Update();

                    return true;
                }
                else
                {
                    if (verbose)
                        ProgramLog.Error.Log("Specified database file not found");
                }
            }
            catch (Exception e)
            {
                if (verbose)
                    ProgramLog.Log(e, "Specified database file not found");
            }

            return false;
        }
    }
}

