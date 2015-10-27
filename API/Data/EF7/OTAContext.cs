#if ENTITY_FRAMEWORK_7
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace OTA.Data.EF7
{
    public class OTATest
    {
        [Key]
        public int Id { get; set; }
        public string DataString { get; set; }
    }

    public enum SqliteCopyResult
    {
        Ok,
        DirectoryNotFound,
        FileMissing
    }

    public enum DbSupport
    {
        Sqlite,
        SqlServer,
        MySql
    }

    public static class DbContextExtensions
    {
        /// <summary>
        /// Attempt to copy sqlite files from Data/Sqlite/[x86/x64]/* to where they should be.
        /// </summary>
        public static SqliteCopyResult TryCopySqliteDependencies(this DbContext ctx)
        {
            var path = System.IO.Path.Combine(Globals.DataPath, "Sqlite", Environment.Is64BitProcess ? "x64" : "x86");
            if (!System.IO.Directory.Exists(path)) return SqliteCopyResult.DirectoryNotFound;

            //Copy the new platform files
            foreach (var file in new string[] { "sqlite3.dll", "sqlite3.def" })
            {
                var fl = System.IO.Path.Combine(path, file);
                if (!System.IO.File.Exists(fl))
                    return SqliteCopyResult.FileMissing;

                //Remove the existing, incase the platform changed
                //This actually can even occur when you run in debug mode
                //on a x64 machine, vshost can be x86.
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);

                System.IO.File.Copy(fl, file);
            }

            return SqliteCopyResult.Ok;
        }
    }

    //public class OTAContext : DbContext
    //{
    //    public DbSet<OTATest> OtaTests { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ota;Trusted_Connection=True;");
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //    }
    //}
}
#endif