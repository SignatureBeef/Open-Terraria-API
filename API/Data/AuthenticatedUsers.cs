using System;
using OTA.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OTA.Data
{
    /// <summary>
    /// Default OTA user information
    /// </summary>
    public class DbPlayer
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool Operator { get; set; }

        public DateTime DateAddedUTC { get; set; }

        public override string ToString()
        {
            return String.Format("[UserDetails: Id {3}, Username: '{0}', Password: '{1}', Operator: {2}]", Username, Password, Operator, Id);
        }

        /// <summary>
        /// Compares a username & password to the current instance
        /// </summary>
        /// <returns><c>true</c>, if password was compared, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public bool ComparePassword(string username, string password)
        {
            var hs = AuthenticatedUsers.Hash(username, password);

            return hs.Equals(Password);
        }
    }

    /// <summary>
    /// Authenticated users.
    /// </summary>
    /// <remarks></remarks>
    public static class AuthenticatedUsers
    {
        public const String SQLSafeName = "tdsm";

        internal static string Hash(string username, string password)
        {
            var hash = SHA256.Create();
            var sb = new StringBuilder(64);
            var bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(username + ":" + password));
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// The default user table
        /// </summary>
        public class UserTable
        {
            public const String TableName = "users";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String Username = "Username";
                public const String Password = "Password";
                public const String Operator = "Operator";
                public const String DateAdded = "DateAdded";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.Username, typeof(String), 255),
                new TableColumn(ColumnNames.Password, typeof(String), 255),
                new TableColumn(ColumnNames.Operator, typeof(Boolean)),
                new TableColumn(ColumnNames.DateAdded, typeof(DateTime))
            };

            /// <summary>
            /// Checks if the table exists
            /// </summary>
            public static bool Exists()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return Storage.Execute(bl);
                }
            }

            /// <summary>
            /// Creates the table
            /// </summary>
            public static bool Create()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return Storage.ExecuteNonQuery(bl) > 0;
                }
            }
        }

        internal static void Initialise()
        {
            if (!UserTable.Exists())
            {
                ProgramLog.Admin.Log("Common user table does not exist and will now be created");
                UserTable.Create();
            }
        }

        /// <summary>
        /// Gets the user count.
        /// </summary>
        /// <value>The user count.</value>
        public static int UserCount
        {
            get
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl
                        .Select()
                        .Count()
                        .From(UserTable.TableName);

                    return Storage.ExecuteScalar<Int32>(bl);
                }
            }
        }

        /// <summary>
        /// Checks if a user exists
        /// </summary>
        /// <returns><c>true</c>, if exists was usered, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool UserExists(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Select().Count().From(UserTable.TableName).Where(new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<Int64>(bl) > 0;
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        /// <returns>The user password.</returns>
        /// <param name="username">Username.</param>
        public static string GetUserPassword(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[] { UserTable.ColumnNames.Password }, new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<String>(bl);
            }
        }

        /// <summary>
        /// Gets the user from the database by name
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="username">Username.</param>
        public static DbPlayer GetUser(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[]
                    {
                        UserTable.ColumnNames.Id,
                        UserTable.ColumnNames.Username,
                        UserTable.ColumnNames.Password, 
                        UserTable.ColumnNames.Operator 
                    }, new WhereFilter(UserTable.ColumnNames.Username, username));

                var res = Storage.ExecuteArray<DbPlayer>(bl);
                if (res != null && res.Length > 0)
                    return res[0];

                return null;
            }
        }

        private struct FUBP
        {
            public string Username;
        }

        /// <summary>
        /// Finds a list of users matching a prefix
        /// </summary>
        /// <returns>The users by prefix.</returns>
        /// <param name="search">Search.</param>
        public static string[] FindUsersByPrefix(string search)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[]
                    {
                        UserTable.ColumnNames.Username,
                    }, new WhereFilter(UserTable.ColumnNames.Username, search + '%', WhereExpression.Like));

                return Storage.ExecuteArray<FUBP>(bl).Select(x => x.Username).ToArray();
            }
        }

        /// <summary>
        /// Removes a user from the database by name
        /// </summary>
        /// <returns><c>true</c>, if user was deleted, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool DeleteUser(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Delete(UserTable.TableName, new WhereFilter[]
                    {
                        new WhereFilter(UserTable.ColumnNames.Username, username)
                    });
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <returns><c>true</c>, if user was created, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool CreateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.InsertInto(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Username, username),
                        new DataParameter(UserTable.ColumnNames.Password, hs),
                        new DataParameter(UserTable.ColumnNames.Operator, op),
                        new DataParameter(UserTable.ColumnNames.DateAdded, DateTime.Now)
                    });
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool UpdateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Password, hs),
                        new DataParameter(UserTable.ColumnNames.Operator, op)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public static bool UpdateUser(string username, string password)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Password, hs)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool UpdateUser(string username, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Operator, op)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }
    }
}

