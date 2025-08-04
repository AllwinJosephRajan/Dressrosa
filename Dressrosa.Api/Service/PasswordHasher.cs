using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Api.Services
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            if (password != null)
            {
                var hasher = SHA256.Create();
                var hash = hasher.ComputeHash(Encoding.Default.GetBytes(password));
                return Convert.ToBase64String(hash);
            }

            return string.Empty;
        }
        public bool VerifyHashedPassword(string passwordHash, string password)
        {
            if (passwordHash != null && password != null)
            {
                var hash = HashPassword(password);
                return hash == passwordHash;
            }

            return false;
        }
    }
}
