using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    internal static class PasswordInterface
    {
        const int keySize = 64;
        const int iterations = 350000;
        static private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        static public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        static public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }

        /// <summary>
        /// Method <c>VerifyIfpasswordIsSafe</c> check if password is safe
        /// </summary>
        /// <param name="password"></param>
        /// <returns>bool</returns>
        static public bool VerifyIfpasswordIsSafe(string password)
        {
            char[] special_chars = "!@#$%^&*()_+-=<>/\\".ToCharArray();

            if (password == null) { return false; } // check if password is not empty
            if (password.Length >= 8) { return false; } // check if password has min 8 letters
            if (!password.Any(char.IsLower) ) { return false; } // check if password has min one lower letter
            if (!password.Any(char.IsUpper)) { return false; } // check if password has min one upper letter
            if (!password.Contains(" ")) { return false; } // check if password doesn't contain empty space

            foreach ( char c in special_chars )
            {
                if (!password.Contains(c)) { return true; } // check if password has min one special character
            }
            return false;

        }

    }
}
