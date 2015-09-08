using System.Data.Entity;
using OTA.Data.Entity.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using OTA.Data.Entity;

namespace OTA.Data
{
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

        public DbSet<APIAccountRole> APIAccountsRoles { get; set; }

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

            builder.Entity<APIAccountRole>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}

