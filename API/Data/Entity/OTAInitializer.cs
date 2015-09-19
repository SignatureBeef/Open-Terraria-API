using System;
using System.Data.Entity;

namespace OTA.Data.Entity
{
    public class OTAInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        public virtual void InitializeDatabase(T context)
        {
            //All instances of DbContext's for the current database must be done by now
            //So now we can fire an event to populate the database with default values (if any)
            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                plg.NotifyDatabaseCreated();
            }
        }
    }
}

