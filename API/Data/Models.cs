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

    class EFConfiguration : DbConfiguration
    {
        public static string ProviderName { get; set; }

        public static DbProviderServices ProviderService { get; set; }

        public static DbProviderFactory ProviderFactory { get; set; }

        public EFConfiguration()
        {
            if (ProviderFactory != null && !String.IsNullOrEmpty(ProviderName))
            {
                this.SetProviderFactory(ProviderName, ProviderFactory);
            }
            if (ProviderService != null && !String.IsNullOrEmpty(ProviderName))
            {
                this.SetProviderServices(ProviderName, ProviderService);
            }
            if (ProviderFactory != null)
            {
                this.SetDefaultConnectionFactory(OTAConnectionFactory.Instance);
            }
            this.SetProviderFactoryResolver(new SQLiteProviderFactoryResolver());
        }
    }

    public class SQLiteProviderFactoryResolver : IDbProviderFactoryResolver
    {
        public DbProviderFactory ResolveProviderFactory(DbConnection connection)
        {
            return DbProviderFactories.GetFactory(connection);
        }
    }

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

    /// <summary>
    /// The connection context for talking to an OTA database
    /// </summary>
    [DbConfigurationType(typeof(EFConfiguration))] 
    public class OTAContext : DbContext // IdentityDbContext<IdentityUser>
    {
        public OTAContext() : this(ConnectionManager.ConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OTA.Data.Models.OTAContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name or connection string. Default is terraria_ota</param>
        public OTAContext(string nameOrConnectionString = "terraria_ota") : base(nameOrConnectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
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

            if (this.Database.Connection.GetType().Name == "SQLiteConnection") //Since we support SQLite as default, let's use this hack...
            {
                Database.SetInitializer(new SqliteContextInitializer<OTAContext>(builder));
            }

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
            //Find the database file name from the connection
            var matches = System.Text.RegularExpressions.Regex.Match(context.Database.Connection.ConnectionString, "Data Source=(.*?);");
            if (matches != null && matches.Length > 0)
            {
                while (matches.Success)
                {
                    if (matches.Groups.Count > 1)
                    {
                        var db = matches.Groups[1].Value;
                        if (File.Exists(db)) return;
                    }

                    matches = matches.NextMatch();
                }  
            }
//            if (context.Database.Exists())
//                return;

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
                return new OTAContext(ConnectionString);
            }
        }

        static T LoadInstanceType<T>(Type type) where T : class
        {
            T instance = default(T);

            //Typically there is a general rule of thumb about an 'Instance' static field on the providers factory class
            //Let's try to use it if we can find one in the case they use it in the library.
            var ist = type.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (ist != null)
            {
                instance = ist.GetValue(null) as T;
            }

//            if (factory == null) untested
//            {
//                var pst = type.GetProperty("Instance", System.Reflection.BindingFlags.Public);
//                if (pst.CanRead)
//                {
//                    factory = pst.GetGetMethod().Invoke(null, null) as DbProviderFactory;
//                }
//            }

            if (instance == null)
            {
                instance = Activator.CreateInstance(type) as T;
            }

            return instance;
        }

        /// <summary>
        /// Prepares the database for connection.
        /// </summary>
        /// <remarks>It will load the configuration using the assembly specified.</remarks>
        /// <param name="assemblyName">Assembly name containing the configuration class.</param>
        /// <param name="verbose">If set to <c>true</c> verbose.</param>
        public static bool PrepareFromAssembly(string assemblyName, bool verbose = false)
        {
            try
            {
                EFConfiguration.ProviderName = assemblyName;

                //Find the assembly by name that was pre-loaded from the Libraries folder
                var providers = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(x => x.FullName.StartsWith(assemblyName));
                
                foreach (var provider in providers)
                {
                    System.Reflection.TypeInfo inf;

                    //Find the assembly by name that was pre-loaded from the Libraries folder
                    if (EFConfiguration.ProviderFactory == null)
                    {
                        inf = provider.DefinedTypes.Where(x => typeof(System.Data.Common.DbProviderFactory).IsAssignableFrom(x)).FirstOrDefault();
                        if (inf != null)
                        {
                            EFConfiguration.ProviderFactory = LoadInstanceType<DbProviderFactory>(inf.AsType());
                        }
                    }
                    if (EFConfiguration.ProviderFactory == null)
                    {
                        inf = provider.DefinedTypes.Where(x => typeof(DbProviderServices).IsAssignableFrom(x)).FirstOrDefault();
                        if (inf != null)
                        {
                            EFConfiguration.ProviderService = LoadInstanceType<DbProviderServices>(inf.AsType());
                        }
                    }

                    System.Data.Entity.DbConfiguration.LoadConfiguration(provider);

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
                }

                //Load the configuration from the found provider
                DbConfiguration.Loaded += (sender, args) =>
                {
                    //This replacement will ensure we (OTA) can dictate the connection string
                    if (EFConfiguration.ProviderFactory != null) args.ReplaceService<DbProviderFactory>((s, a) => EFConfiguration.ProviderFactory); 
                    if (EFConfiguration.ProviderService != null) args.ReplaceService<DbProviderServices>((s, a) => EFConfiguration.ProviderService); 
                    if (EFConfiguration.ProviderFactory != null) args.ReplaceService<IDbConnectionFactory>((s, a) => OTAConnectionFactory.Instance); 
                };
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

