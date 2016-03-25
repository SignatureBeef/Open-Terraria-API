using Dapper;
using OTA.Data.Dapper.Mappers;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace OTA.Data.Dapper.Extensions
{
    public static class DapperQueryExtensions
    {
        public static bool Any<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return cnn.Count<T>(param, transaction, commandTimeout, commandType) > 0;
        }

        public static void AddClause(this StringBuilder builder, object param)
        {
            builder.Append(" where ");

            var and = false;
            foreach (var prop in param.GetType().GetProperties())
            {
                if (and) builder.Append(" and ");
                builder.Append(ColumnMapper.Enclose(prop.Name));
                builder.Append("=");
                builder.Append("@");
                builder.Append(prop.Name);

                if (!and) and = true;
            }
        }

        public static long Count<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select count(*) from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.ExecuteScalar<long>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static IEnumerable<T> Where<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.Query<T>(sql.ToString(), param, transaction, buffered, commandTimeout, commandType);
        }

        public static int Delete<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("delete from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.Execute(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static async Task<IEnumerable<T>> WhereAsync<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return await cnn.QueryAsync<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static T Single<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.QuerySingle<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static T SingleOrDefault<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.QuerySingleOrDefault<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static T FirstOrDefault<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.QueryFirstOrDefault<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return await cnn.QueryFirstOrDefaultAsync<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }

        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(TableMapper.TypeToName<T>());
            sql.AddClause(param);

            return cnn.QueryFirstOrDefault<T>(sql.ToString(), param, transaction, commandTimeout, commandType);
        }
    }
}
