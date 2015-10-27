#if ENTITY_FRAMEWORK_7
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTA.Data.EF7.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
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

            return builder;
        }
#endregion

        /// <summary>
        /// Attempt to load an OTA supported database.
        /// </summary>
        public static DbContextOptionsBuilder UseDynamic(this DbContextOptionsBuilder builder, string connectionType, string connectionString)
        {
            switch (connectionType)
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