#if ENTITY_FRAMEWORK_7
using System;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Operations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity;

namespace OTA.Data.EF7.Extensions
{
    public static class MigrationExtensions
    {
        /// <summary>
        /// Adds a identity/auto increment column annotation, dependant on the provider.
        /// </summary>
        public static OperationBuilder<AddColumnOperation> AutoIncrement(this OperationBuilder<AddColumnOperation> builder, string activeProvider)
        {
            //TODO verify what the migration passes through
            if (activeProvider == "sqlite")
                builder.Annotation("Sqlite:Autoincrement", true);
            else if (activeProvider == "sqlserver")
                builder.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn); //TODO reflect this

            return builder;
        }
    }
}
#endif