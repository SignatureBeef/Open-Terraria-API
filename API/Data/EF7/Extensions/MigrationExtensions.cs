#if ENTITY_FRAMEWORK_7
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Migrations.Operations.Builders;

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
            {
                var type = System.Type.GetType("SqlServerValueGenerationStrategy");
                var value = System.Enum.Parse(type, "IdentityColumn");
                builder.Annotation("SqlServer:ValueGenerationStrategy", value);
            }
            //TODO postgres migration AutoIncrement

            return builder;
        }
    }
}
#endif