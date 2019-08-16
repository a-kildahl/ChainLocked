using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ChainLocked.Cryptography
{
    public static class Hasher
    {
        /// <summary>
        /// Computes a hex encoded SHA256 hash.
        /// </summary>
        /// <param name="data">The string to hash.</param>
        /// <returns>The SHA256 hash.</returns>
        public static string ComputeHash(string data)
        {
            using (SHA256 alg = SHA256.Create())
            {
                byte[] dataBuffer = Encoding.UTF8.GetBytes(data);
                byte[] resultBuffer = alg.ComputeHash(dataBuffer);
                return BitConverter.ToString(resultBuffer).Replace("-", "").ToUpper();
            }
        }

    }
}
