using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using OTA.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTA.Data.Dapper.Extensions;
using Dapper;
using System.Data;

namespace Dapper_Tests
{
    [OTAVersion(1, 0)]
    public class DapperTestsPlugin : BasePlugin
    {
        public DapperTestsPlugin()
        {
            this.Author = "DeathCradle";
            this.Description = "Dapper testing plugin";
            this.Name = "Dapper tests";
            this.Version = "1";
        }

        protected override void Enabled()
        {
            base.Enabled();

            using (var conn = OTA.Data.DatabaseFactory.CreateConnection())
            {
                conn.Open();
                var res = conn.Where<DapperTests>(new { Name = "Testing" });
                var asd = "";
            }
        }
    }

    public class DapperTests
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime? DateAdded { get; set; }
    }

    [Migration(1)]
    public class CreateAndSeedMigration : Migration
    {
        public override void Up()
        {
            var dapperTests = this.Create.Table<DapperTests>();

            dapperTests.WithColumn("Id")
                .AsInt32()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            dapperTests.WithColumn("Name")
                .AsString()
                .WithDefaultValue(String.Empty)
                .NotNullable();

            this.Insert.IntoTable<DapperTests>()
                .Row(new
                {
                    Name = "Testing"
                });
        }

        public override void Down()
        {

        }
    }

    [Migration(2)]
    public class AlterMigration : Migration
    {
        public override void Up()
        {
            this.Alter.Table<DapperTests>()
                .AddColumn("DateAdded")
                .AsDateTime()
                .Nullable();
            //.WithDefault(SystemMethods.CurrentDateTime); damn you sqlite

            this.Update.Table<DapperTests>()
                .Set(new { DateAdded = DateTime.Now })
                .Where(new { DateAdded = (DateTime?)null });

            this.Insert.IntoTable<DapperTests>()
                .Row(new
                {
                    Name = "Date added",
                    DateAdded = DateTime.Now
                });
        }

        public override void Down()
        {

        }
    }
}
