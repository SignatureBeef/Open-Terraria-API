#if ENTITY_FRAMEWORK_6
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OTA.Plugin;

namespace OTA.Data.EF6
{
    /// <summary>
    /// The connection context for talking to an OTA database
    /// </summary>
    //    [DbConfigurationType(typeof(EFConfiguration))] 
    public class OTAContext : DbContext // IdentityDbContext<IdentityUser>
    {
        public static DbConfiguration Config;

        internal static bool ProbeSuccess { get; set; }

        public static bool HasConnection => ProbeSuccess && System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionNameOrString] != null;

        //TODO fix this hack - seems there is no IndexOf function in SQLite, so we need something in the ADO/EF dll for this.
        //Maybe EF7 solves this (?)
        public static bool IsSQLite
        {
            get;
            private set;
        }

        const string DefaultConnection = "terraria_ota";

        public static string ConnectionNameOrString { get; set; }

        static OTAContext()
        {
            ConnectionNameOrString = DefaultConnection;
        }

        public OTAContext() : this(ConnectionNameOrString) //ConnectionManager.ConnectionString)
        {
//            if (null != Config)
//            {
//                System.Console.Write("Setting DB Config...");
//                DbConfiguration.SetConfiguration(Config);
//                System.Console.WriteLine("Done");
//            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OTA.Data.Models.OTAContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name or connection string. Default is terraria_ota</param>
        public OTAContext(string nameOrConnectionString = DefaultConnection) : base(nameOrConnectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
//            builder.HasDefaultSchema("ota");

            Logging.ProgramLog.Admin.Log("Initialising database for provider {0}", 
                System.Configuration.ConfigurationManager.ConnectionStrings[OTAContext.ConnectionNameOrString].ProviderName);

            builder.Conventions.Remove<PluralizingTableNameConvention>();
//            builder.Entity<HistoryRow>()
//                .Property(h => h.MigrationId)
//                .HasMaxLength(100)
//                .IsRequired();
//            builder.Entity<HistoryRow>()
//                .Property(h => h.ContextKey)
//                .HasMaxLength(200)
//                .IsRequired();


//            builder.Entity<HistoryRow>()
//                .ToTable("__MigrationHistory", "ota")
//                .HasKey(x => x.MigrationId)
//                .Property(h => h.MigrationId)
//                .HasMaxLength(100)
//                .IsRequired();
//            builder.Entity<HistoryRow>()
//                .Property(h => h.ContextKey)
//                .HasMaxLength(200)
//                .IsRequired();

            if (this.Database.Connection.GetType().Name == "SQLiteConnection") //Since we support SQLite as default, let's use this hack...
            {
                Database.SetInitializer(new OTA.Data.EF6.Config.SQLite.SqliteContextInitializer<OTAContext>(builder));
                IsSQLite = true;
            }
            else
            {
                Database.SetInitializer(new OTA.Data.EF6.Config.OTAInitializer<OTAContext>());
                IsSQLite = false;
            }

            //Allow plugins to apply to our database 
            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                plg.NotifyDatabaseInitialising(builder);
            }
        }
    }
}

#endif