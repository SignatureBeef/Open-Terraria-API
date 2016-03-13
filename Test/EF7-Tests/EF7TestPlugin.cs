using Microsoft.Data.Entity;
using OTA.Data.EF7;
using OTA.Logging;
using OTA.Plugin;
using System.Linq;

namespace EF7_Tests
{
    [OTAVersion(1, 0)]
    public class EF7TestPlugin : BasePlugin
    {
        public static LogChannel Log = new LogChannel("EF7", System.ConsoleColor.Magenta, System.Diagnostics.TraceLevel.Verbose);

        public EF7TestPlugin()
        {
            this.Name = "EF7 Test Plugin";
            this.Version = "1.0";
            this.Description = "Test plugin for EF7";
        }

        protected override void Initialized(object state)
        {
            base.Initialized(state);
            Log.Log("Initialised");
        }

        protected override void DatabaseCreated()
        {
            base.DatabaseCreated();
            Log.Log("DatabaseCreated");

            using (var ctxA = new EF7AContext())
            using (var ctxB = new EF7BContext())
            {
                if (!ctxA.EF7A.Any(x => x.Name == "Test"))
                {
                    ctxA.EF7A.Add(new EF7()
                    {
                        Name = "Test"
                    });
                }

                var item = ctxB.Tick.SingleOrDefault(x => x.Name == "Test");
                if (item == null)
                {
                    ctxB.Tick.Add(new Tick()
                    {
                        Name = "Test",
                        Value = 1
                    });
                }
                else
                {
                    item.Value++;
                }

                ctxA.SaveChanges();
                ctxB.SaveChanges();
            }
        }
    }

    public class EF7AContext : OTAContext
    {
        public DbSet<EF7> EF7A { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EF7>().HasKey(c => c.Id);
        }
    }

    public class EF7BContext : OTAContext
    {
        public DbSet<Tick> Tick { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tick>().HasKey(c => c.Id);
        }
    }

    public class EF7
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Tick
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
