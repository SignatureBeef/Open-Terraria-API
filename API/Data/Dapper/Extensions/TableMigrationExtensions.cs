using Dapper.Contrib.Extensions;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;
using FluentMigrator.Builders.Rename;
using FluentMigrator.Builders.Rename.Table;
using FluentMigrator.Builders.Schema;
using FluentMigrator.Builders.Schema.Table;
using FluentMigrator.Builders.Update;
using OTA.Data.Dapper.Mappers;
using System;

namespace OTA.Data.Dapper.Extensions
{
    public static class TableMigrationExtensions
    {
        public static ICreateTableWithColumnOrSchemaOrDescriptionSyntax Table<T>(this ICreateExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }

        public static IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax Table<T>(this IAlterExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }

        public static IInSchemaSyntax Table<T>(this IDeleteExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }

        public static IInsertDataOrInSchemaSyntax IntoTable<T>(this IInsertExpressionRoot root) where T : class
        {
            return root.IntoTable(TableMapper.TypeToName<T>());
        }

        public static IRenameTableToOrInSchemaSyntax Table<T>(this IRenameExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }

        public static IInSchemaSyntax To<T>(this IRenameTableToSyntax root) where T : class
        {
            return root.To(TableMapper.TypeToName<T>());
        }

        public static ISchemaTableSyntax Table<T>(this ISchemaExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }

        public static IUpdateSetOrInSchemaSyntax Table<T>(this IUpdateExpressionRoot root) where T : class
        {
            return root.Table(TableMapper.TypeToName<T>());
        }
    }
}
