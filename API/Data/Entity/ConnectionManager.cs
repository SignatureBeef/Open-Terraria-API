using System;
using OTA.Data;
using System.Linq;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using OTA.Logging;

namespace OTA.Data.Entity
{
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
//                EFConfiguration.ProviderName = assemblyName;

                //Find the assembly by name that was pre-loaded from the Libraries folder
                var providers = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(x => x.FullName.StartsWith(assemblyName));

                foreach (var provider in providers)
                {
                    Console.WriteLine(provider.FullName);
                    System.Reflection.TypeInfo inf;

//                    //Find the assembly by name that was pre-loaded from the Libraries folder
//                    if (EFConfiguration.ProviderFactory == null)
//                    {
//                        inf = provider.DefinedTypes.Where(x => typeof(System.Data.Common.DbProviderFactory).IsAssignableFrom(x)).FirstOrDefault();
//                        if (inf != null)
//                        {
////                            EFConfiguration.ProviderFactory = LoadInstanceType<DbProviderFactory>(inf.AsType());
//                        }
//                    }
//                    if (EFConfiguration.ProviderFactory == null)
//                    {
//                        inf = provider.DefinedTypes.Where(x => typeof(DbProviderServices).IsAssignableFrom(x)).FirstOrDefault();
//                        if (inf != null)
//                        {
////                            EFConfiguration.ProviderService = LoadInstanceType<DbProviderServices>(inf.AsType());
//                        }
//                    }
                    //                    if (EFConfiguration.ProviderConfiguration == null)
                    //                    {
                    inf = provider.DefinedTypes.Where(x => typeof(DbConfiguration).IsAssignableFrom(x)).FirstOrDefault();
                    if (inf != null)
                    {
//                        OTAContext.Config = LoadInstanceType<DbConfiguration>(inf.AsType());
                        DbConfiguration.SetConfiguration(LoadInstanceType<DbConfiguration>(inf.AsType()));
//                        DbConfiguration.SetConfiguration(OTAContext.Config);
//                        EFConfiguration.ProviderService = null;
//                        EFConfiguration.ProviderFactory = null;
//                        EFConfiguration.ProviderName = null;
                        return true;
                    }
                    //                    }

//                    System.Data.Entity.DbConfiguration.LoadConfiguration(provider);

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

//                //Load the configuration from the found provider
//                DbConfiguration.Loaded += (sender, args) =>
//                {
//                    //This replacement will ensure we (OTA) can dictate the connection string
//                    if (EFConfiguration.ProviderFactory != null) args.ReplaceService<DbProviderFactory>((s, a) => EFConfiguration.ProviderFactory); 
//                    if (EFConfiguration.ProviderService != null) args.ReplaceService<DbProviderServices>((s, a) => EFConfiguration.ProviderService); 
//                    if (EFConfiguration.ProviderFactory != null) args.ReplaceService<IDbConnectionFactory>((s, a) => OTAConnectionFactory.Instance); 
//                    //                    if (EFConfiguration.ProviderConfiguration != null) args.ReplaceService<DbConfiguration>((s, a) => EFConfiguration.ProviderConfiguration); 
//                };
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

