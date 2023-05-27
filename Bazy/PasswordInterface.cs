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
        // https://code-maze.com/csharp-hashing-salting-passwords-best-practices/
        const int keySize = 64;
        const int iterations = 350000;
        static private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        /// <summary>
        /// Method <c>HashPasword(<paramref name="password"/>,<paramref name="salt"/>)</c>
        /// generates hash and salt from given password.
        /// </summary>
        /// <param name="password">Password given by user to be hashed</param>
        /// <param name="salt">Salt used to generate hash (needs to be safed with password)</param>
        /// <returns>Hash and Salt</returns>
        
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

        /// <summary>
        /// Method <c>VerifyPassword(<paramref name="hash"/>,<paramref name="password"/>,<paramref name="salt"/>)</c>
        /// checks if password given by the user is the same with the saved password that has been hashed
        /// </summary>
        /// <param name="password">Password provided by user that you want to check if hash</param>
        /// <param name="hash">Hash for the user that you want to heck password</param>
        /// <param name="salt">Salt for the user that you want to heck password</param>
        /// <returns></returns>
        static public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            byte[] bytes = Convert.FromBase64String(hash);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }

        /// <summary>
        /// Method <c>VerifyIfpasswordIsSafe</c> verifies if a password is safe according to the specified criteria.
        /// Safe password criteria:
        /// <list type="bullet">
        ///    <item>Min 8 letters</item>
        ///    <item>Min 1 low letter</item>
        ///    <item>Min 1 upper letter</item>
        ///    <item>Doesn't contain any whitespaces</item>
        ///    <item>Has one special character</item>
        /// </list>
        /// </summary>
        /// <param name="password">Password to be verified</param>
        /// <returns>Returns True if the password is safe, False otherwise.</returns>
        static public bool VerifyIfpasswordIsSafe(string password)
        {
            char[] special_chars = "!@#$%^&*()_+-=<>/\\".ToCharArray();

            if (password == null) { return false; }
            if (password.Length < 8) { return false; }
            if (!password.Any(char.IsLower) ) { return false; }
            if (!password.Any(char.IsUpper)) { return false; }
            if (password.Contains(" ")) { return false; }
            foreach ( char c in special_chars )
            {
                if (password.Contains(c)) { return true; }
            }
            return false;
        }

    }
}
