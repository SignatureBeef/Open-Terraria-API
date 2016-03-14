#if ENTITY_FRAMEWORK_7
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Entity;
using OTA.Extensions;
using System.Linq;
using OTA.Data.EF7.Extensions;

namespace OTA.Data.EF7
{
    public enum SqliteCopyResult
    {
        Ok,
        DirectoryNotFound,
        FileMissing,
        AccessException
    }

    public static class OTAContextFactory
    {
        public static string ConnectionProvider { get; set; }
        public static string ConnectionString { get; set; }

        public static OTAContext Create()
        {
            if (!String.IsNullOrEmpty(ConnectionProvider))
                return new OTAContext(ConnectionProvider, ConnectionString);

            return null;
        }
    }

    public class OTAContext : DbContext
    {
        public string ConnectionProvider { get; set; }
        public string ConnectionString { get; set; }

        protected OTAContext()
        {
            this.ConnectionProvider = OTAContextFactory.ConnectionProvider;
            this.ConnectionString = OTAContextFactory.ConnectionString;
        }

        public OTAContext(string connectionProvider, string connectionString)
        {
            this.ConnectionProvider = connectionProvider;
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ota;Trusted_Connection=True;");
            //optionsBuilder.UseDynamic("sqlserver", "Server=.\\SQLEXPRESS;Database=ota;Trusted_Connection=True;");
            //optionsBuilder.UseDynamic("sqlite", "Data Source=database.sqlite");
            optionsBuilder.UseDynamic(ConnectionProvider, ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //This determines if the code running is the root context
            if (this.GetType() == typeof(OTAContext))
            {
                InitialiseSubContexts(modelBuilder);
            }
        }

        /// <summary>
        /// Import plugin context DbSets into the model being created
        /// </summary>
        /// <param name="modelBuilder"></param>
        internal static void InitialiseSubContexts(ModelBuilder modelBuilder)
        {
            var type = typeof(DbContext);
            foreach (var plg in Plugin.PluginManager.EnumeratePlugins)
            {
                foreach (var ctx in plg.Assembly.GetTypesLoaded()
                    .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
                {
                    Logging.ProgramLog.Debug.Log("Importing context {0} from {1}", ctx.Name, plg.Name);

                    var dbContext = (DbContext)Activator.CreateInstance(ctx);

                    var mth = ctx.GetMethod("OnModelCreating", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mth.Invoke(dbContext, new object[] { modelBuilder });
                    dbContext.Dispose();
                }
            }
        }
    }
}
#endif