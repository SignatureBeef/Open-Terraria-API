using System;
using Microsoft.Owin.Security.DataProtection;
using System.Security.Cryptography;
using System.IO;

namespace OTA.Web.Internals
{
    /// <summary>
    /// A mono compatible data protector for use with OWIN
    /// </summary>
    internal class MonoDataProtector : IDataProtector
    {
        private string[] _purposes;
        const String DefaultPurpose = "ota-web-dp";

        public MonoDataProtector()
        {
            _purposes = null;
        }

        public MonoDataProtector(params string[] purposes)
        {
            _purposes = purposes;
        }

        public byte[] Protect(byte[] data)
        {
            return System.Security.Cryptography.ProtectedData.Protect(data, this.GenerateEntropy(), DataProtectionScope.CurrentUser);
        }

        public byte[] Unprotect(byte[] data)
        {
            return System.Security.Cryptography.ProtectedData.Unprotect(data, this.GenerateEntropy(), DataProtectionScope.CurrentUser);
        }

        byte[] GenerateEntropy()
        {
            using (var hasher = SHA256.Create())
            {
                using (var ms = new MemoryStream())
                using (var cr = new CryptoStream(ms, hasher, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cr))
                {
                    //Default purpose 
                    sw.Write(DefaultPurpose);

                    if (_purposes != null)
                        foreach (var purpose in _purposes)
                        {
                            sw.Write(purpose);
                        }
                }

                return hasher.Hash;
            }
        }
    }
}

