using System;

namespace OTA.Data.Entity.Models
{
    public class APIAccount
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password
        {
            set
            {
                PasswordFormat = PasswordFormat.SHA256;
                PasswordHash = Hash_256(value);
            }
        }

        public string PasswordHash { get; set; }

        public PasswordFormat PasswordFormat { get; set; }

        public bool ComparePassword(string password)
        {
            if (PasswordFormat.SHA256 == PasswordFormat)
                return Hash_256(password).Equals(PasswordHash);

            return false;
        }

        private string Hash_256(string password)
        {
            using (var hsr = System.Security.Cryptography.SHA256.Create())
            {
                var data = System.Text.Encoding.UTF8.GetBytes(password);
                var hashed = hsr.ComputeHash(data);

                var sb = new System.Text.StringBuilder();
                foreach (var b in hashed)
                    sb.AppendFormat("{0:x2}", b);

                return sb.ToString();
            }
        }
    }
}

