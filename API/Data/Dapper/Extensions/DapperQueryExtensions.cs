using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OTA.Data.Dapper.Extensions
{
    public static class DapperQueryExtensions
    {
        public static IEnumerable<T> Where<T>(this IDbConnection cnn, object param, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            var sql = new StringBuilder();

            sql.Append("select * from ");
            sql.Append(typeof(T).Name);
            sql.Append(" where ");
            foreach (var prop in param.GetType().GetProperties())
            {
                sql.Append(prop.Name);
                sql.Append("=");
                sql.Append("@");
                sql.Append(prop.Name);
            }

            return cnn.Query<T>(sql.ToString(), param, transaction, buffered, commandTimeout, commandType);
        }
    }
}
