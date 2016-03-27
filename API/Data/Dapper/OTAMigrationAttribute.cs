using FluentMigrator;
using System;

namespace OTA.Data.Dapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OTAMigrationAttribute : MigrationAttribute
    {
        public OTAMigrationAttribute(long version, Type plugin)
            : this(version, TransactionBehavior.Default, plugin)
        {
        }

        public OTAMigrationAttribute(long version, string description, Type plugin)
            : this(version, TransactionBehavior.Default, description, plugin)
        {
        }

        public OTAMigrationAttribute(long version, TransactionBehavior transactionBehavior, Type plugin)
            : this(version, transactionBehavior, null, plugin)
        {
        }

        public OTAMigrationAttribute(long version, TransactionBehavior transactionBehavior, string description, Type plugin)
            : base(GetVersion(version, plugin), transactionBehavior, description)
        {

        }

        private static long GetVersion(long version, Type plugin)
        {

            return plugin.FullName.GetHashCode() + version;
        }
    }
}
