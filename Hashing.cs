using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PasteBin
{
    public static class Hashing
    {
        static byte[] salt = new byte[128 / 8];

        public static string hash(string pass, string username) 
        {
            salt = Encoding.UTF8.GetBytes(username);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pass,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        }
    }
}