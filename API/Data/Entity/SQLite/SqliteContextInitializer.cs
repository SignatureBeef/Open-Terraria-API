using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;

namespace OTA
{
    //Based off https://gist.github.com/flaub/1968486e1b3f2b9fddaf#file-sqlitecontextinitializer-cs
    public class SqliteContextInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        DbModelBuilder _modelBuilder;

        static readonly Dictionary<String, String> DataTypeMap = new Dictionary<String, String>()
        {
            { "int", "integer" }
        };

        public static string GetDataType(string typeName)
        {
            if (DataTypeMap.ContainsKey(typeName)) return DataTypeMap[typeName];

            return typeName;
        }

        public SqliteContextInitializer(DbModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        //<3 http://romiller.com/2014/04/08/ef6-1-mapping-between-types-tables/
        public static string GetTableName(EntityType type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == type.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                .Single()
                .EntitySetMappings
                .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }

        public void InitializeDatabase(T context)
        {
            //Find the database file name from the connection
            var matches = System.Text.RegularExpressions.Regex.Match(context.Database.Connection.ConnectionString, "Data Source=(.*?);");
            if (matches != null && matches.Length > 0)
            {
                while (matches.Success)
                {
                    if (matches.Groups.Count > 1)
                    {
                        var db = matches.Groups[1].Value;
                        if (File.Exists(db)) return;
                    }

                    matches = matches.NextMatch();
                }
            }
            //            if (context.Database.Exists())
            //                return;

            var model = _modelBuilder.Build(context.Database.Connection);

            using (var xact = context.Database.BeginTransaction())
            {
                try
                {
                    CreateDatabase(context, model);
                    xact.Commit();
                }
                catch (Exception)
                {
                    xact.Rollback();
                    throw;
                }
            }
        }

        class Index
        {
            public string Name { get; set; }

            public string Table { get; set; }

            public List<string> Columns { get; set; }
        }

        private void CreateDatabase(T ctx, DbModel model)
        {
            const string tableTmpl = "CREATE TABLE [{0}] (\n{1}\n);";
            const string columnTmpl = "    [{0}] {1} {2}"; // name, type, decl
            const string primaryKeyTmpl = "    PRIMARY KEY ({0})";
            const string foreignKeyTmpl = "    FOREIGN KEY ({0}) REFERENCES {1} ({2})";
            const string indexTmpl = "CREATE INDEX {0} ON {1} ({2});";

            var indicies = new Dictionary<string, Index>();

            foreach (var type in model.StoreModel.EntityTypes)
            {
                var defs = new List<string>();

                // columns
                foreach (var p in type.Properties)
                {
                    var decls = new HashSet<string>();

                    bool identity = p.StoreGeneratedPattern == System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity;
                    if (identity)
                    {
                        decls.Add("PRIMARY KEY");
                    }
                    else if (!p.Nullable)
                        decls.Add("NOT NULL");

                    var annotations = p.MetadataProperties
                        .Select(x => x.Value)
                        .OfType<IndexAnnotation>();

                    foreach (var annotation in annotations)
                    {
                        foreach (var attr in annotation.Indexes)
                        {
                            if (attr.IsUnique)
                                decls.Add("UNIQUE");

                            if (string.IsNullOrEmpty(attr.Name))
                                continue;

                            Index index;
                            if (!indicies.TryGetValue(attr.Name, out index))
                            {
                                index = new Index
                                {
                                    Name = attr.Name,
                                    Table = type.Name,
                                    Columns = new List<string>(),
                                };
                                indicies.Add(index.Name, index);
                            }
                            index.Columns.Add(p.Name);
                        }
                    }

                    defs.Add(string.Format(columnTmpl, p.Name, GetDataType(p.TypeName), string.Join(" ", decls)));
                }

                // primary keys
                if (type.KeyProperties.Any(x => x.StoreGeneratedPattern != System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity))
                {
                    var keys = type.KeyProperties.Where(x => x.StoreGeneratedPattern != System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern.Identity).Select(x => x.Name);
                    defs.Add(string.Format(primaryKeyTmpl, string.Join(", ", keys)));
                }

                // foreign keys
                foreach (var assoc in model.StoreModel.AssociationTypes)
                {
                    if (assoc.Constraint.ToRole.Name == type.Name)
                    {
                        var thisKeys = assoc.Constraint.ToProperties.Select(x => x.Name);
                        var thatKeys = assoc.Constraint.FromProperties.Select(x => x.Name);
                        defs.Add(string.Format(foreignKeyTmpl,
                                string.Join(", ", thisKeys),
                                assoc.Constraint.FromRole.Name,
                                string.Join(", ", thatKeys)));
                    }
                }

                // create table
                var tableName = GetTableName(type, ctx) ?? type.Name;
                var sql = string.Format(tableTmpl, tableName, string.Join(",\n", defs));
                ctx.Database.ExecuteSqlCommand(sql);
            }

            // create index
            foreach (var index in indicies.Values)
            {
                var columns = string.Join(", ", index.Columns);
                var sql = string.Format(indexTmpl, index.Name, index.Table, columns);
                ctx.Database.ExecuteSqlCommand(sql);
            }
        }
    }
}

