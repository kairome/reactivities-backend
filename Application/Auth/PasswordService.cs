using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Application.Auth
{
    public static class PasswordService
    {
        private const int IterCount = 100000;

        public static (string, string) GenerateSaltHash(string password)
        {
            var salt = GetSalt();
            return (HashPassword(password, salt), Convert.ToBase64String(salt));
        }

        public static bool VerifyPassword(string password, string hash, string salt)
        {
            var newHash = HashPassword(password, Convert.FromBase64String(salt));
            return newHash == hash;
        }

        private static string HashPassword(string password, byte[] salt)
        {
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt, KeyDerivationPrf.HMACSHA256, IterCount, 256 / 8));

            return hash;
        }

        private static byte[] GetSalt()
        {
            var salt = new byte[128 / 8];
            using (var rngService = new RNGCryptoServiceProvider())
            {
                rngService.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}