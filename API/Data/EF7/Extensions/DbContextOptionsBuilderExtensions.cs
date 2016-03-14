#if ENTITY_FRAMEWORK_7
using Microsoft.Data.Entity;
using OTA.Extensions;
using System;
using System.Linq;

namespace OTA.Data.EF7.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Attempt to copy sqlite files from Data/Sqlite/[x86/x64]/* to where they should be.
        /// </summary>
        private static SqliteCopyResult TryCopySqliteDependencies()
        {
            var path = System.IO.Path.Combine(Globals.DataPath, "Sqlite", Environment.Is64BitProcess ? "x64" : "x86");
            if (!System.IO.Directory.Exists(path)) return SqliteCopyResult.DirectoryNotFound;

            //Copy the new platform files
            foreach (var file in new string[] { "sqlite3.dll", "sqlite3.def" })
            {
                var fl = System.IO.Path.Combine(path, file);
                if (!System.IO.File.Exists(fl))
                    return SqliteCopyResult.FileMissing;

                //Remove the existing file in the case the platform changed
                //This actually can even occur when you run in debug mode
                //on a x64 machine, vshost can be x86.
                if (System.IO.File.Exists(file))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return SqliteCopyResult.AccessException;
                    }
                }

                System.IO.File.Copy(fl, file);
            }

            return SqliteCopyResult.Ok;
        }

        //These methods avoid using "UseXYZ" so the type is lazily loaded.
        //If haven't 'actually' tested if the types are loaded automatically
        //But eh, this should work for the time being.
        #region "Dynamic loading"
        /// <summary>
        /// Attempts to use SQLite as the database provider.
        /// </summary>
        internal static DbContextOptionsBuilder TryLoadSqlite(this DbContextOptionsBuilder builder, string connectionString)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypesLoaded())
                .Where(x => x.Name == "SqliteDbContextOptionsBuilderExtensions")
                .SelectMany(t => t.GetMethods())
                .SingleOrDefault(
                    m =>
                        m.Name == "UseSqlite" &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType == typeof(string)
                );

            if (types != null)
            {
                //Ensure the SQLite native dll is where it should be
                TryCopySqliteDependencies();

                //Invoke the method .UseSqlite
                types.Invoke(null, new object[] { builder, connectionString });
            }

            return builder;
        }
        /// <summary>
        /// Attempts to use Microsofts SqlServer as the database provider.
        /// </summary>
        internal static DbContextOptionsBuilder TryLoadSqlServer(this DbContextOptionsBuilder builder, string connectionString)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypesLoaded())
                .Where(x => x.Name == "SqlServerDbContextOptionsExtensions")
                .SelectMany(t => t.GetMethods())
                .SingleOrDefault(
                    m =>
                        m.Name == "UseSqlServer" &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType == typeof(string)
                );

            if (types != null)
            {
                types.Invoke(null, new object[] { builder, connectionString });
            }
            else throw new NotSupportedException("SqlServer dll not loaded.");

            return builder;
        }
        #endregion

        /// <summary>
        /// Attempt to load an OTA supported database.
        /// </summary>
        public static DbContextOptionsBuilder UseDynamic(this DbContextOptionsBuilder builder, string connectionProvider, string connectionString)
        {
            switch (connectionProvider)
            {
                case "sqlite":
                    builder.TryLoadSqlite(connectionString);
                    break;
                case "sqlserver":
                    builder.TryLoadSqlServer(connectionString);
                    break;
            }

            return builder;
        }
    }
}
#endif