using System;
using OTA.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OTA.Data
{
    /// <summary>
    /// Default OTA user information
    /// </summary>
    public class DbPlayer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Operator { get; set; }

        public DateTime DateAddedUTC { get; set; }

        public override string ToString()
        {
            return String.Format("[UserDetails: Id {3}, Name: '{0}', Password: '{1}', Operator: {2}]", Name, Password, Operator, Id);
        }

        /// <summary>
        /// Compares a username & password to the current instance
        /// </summary>
        /// <returns><c>true</c>, if password was compared, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public bool ComparePassword(string name, string password)
        {
            var hs = AuthenticatedUsers.Hash(name, password);

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
        /// Gets the user count.
        /// </summary>
        /// <value>The user count.</value>
        public static int UserCount
        {
            get
            {
                using (var ctx = new OTAContext()) return ctx.Players.Count();
            }
        }

        /// <summary>
        /// Checks if a user exists
        /// </summary>
        /// <returns><c>true</c>, if exists was usered, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool UserExists(string username)
        {
            using (var ctx = new OTAContext())
            {
                return ctx.Players.Any(x => x.Name == username);
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        /// <returns>The user password.</returns>
        /// <param name="username">Username.</param>
        public static string GetUserPassword(string username)
        {
            using (var ctx = new OTAContext())
            {
                return ctx.Players
                    .Where(x => x.Name == username)
                    .Select(x => x.Password)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the user from the database by name
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="username">Username.</param>
        public static DbPlayer GetUser(string username)
        {
            using (var ctx = new OTAContext())
            {
                return ctx.Players
                    .Where(x => x.Name == username)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Finds a list of users matching a prefix
        /// </summary>
        /// <returns>The users by prefix.</returns>
        /// <param name="search">Search.</param>
        public static string[] FindUsersByPrefix(string search)
        {
            using (var ctx = new OTAContext())
            {
                return ctx.Players
                    .Where(x => x.Name.StartsWith(search))
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        /// <summary>
        /// Removes a user from the database by name
        /// </summary>
        /// <returns><c>true</c>, if user was deleted, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static async Task<bool> DeleteUser(string username)
        {
            using (var ctx = new OTAContext())
            {
                var range = ctx.Players.RemoveRange(ctx.Players.Where(x => x.Name == username));

                await ctx.SaveChangesAsync();

                return range.Any();
            }
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <returns><c>true</c>, if user was created, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static async Task<DbPlayer> CreateUser(string username, string password, bool op = false)
        {
            using (var ctx = new OTAContext())
            {
                var player = ctx.Players.Add(new DbPlayer()
                    {
                        Name = username,
                        Password = password,
                        Operator = op,
                        DateAddedUTC = DateTime.UtcNow
                    });

                await ctx.SaveChangesAsync();

                return player;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static async Task<bool> UpdateUser(string username, string password, bool? op = null)
        {
            if (username == null && op == null) throw new InvalidOperationException("You have not specified anything to be updated");
            using (var ctx = new OTAContext())
            {
                var player = ctx.Players.SingleOrDefault(p => p.Name == username);
                if (player == null) throw new InvalidOperationException("Cannot update a non-existent player");

                if (password != null) player.Password = password;
                if (op.HasValue) player.Operator = op.Value;

                await ctx.SaveChangesAsync();

                return true;
            }
        }
    }
}

