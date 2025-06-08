using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebToken.Hash
{
    public class HMAC256TokenHash : ITokenHashGenerator
    {
        private byte[] SecretHashKeyBytes { get; }

        public HMAC256TokenHash(string hashKey) : this(Encoding.ASCII.GetBytes(hashKey)) { }

        public HMAC256TokenHash(byte[] hashKey)
        {
            SecretHashKeyBytes = hashKey;
        }

        public string Hash(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            using var hmac = new HMACSHA256(SecretHashKeyBytes);
            var hashArray = hmac.ComputeHash(byteArray);
            return hashArray.Aggregate("", (s, e) => s + $"{e:x2}", s => s);
        }

        public bool ValidHash(string input)
        {
            return input.Length == 64;
        }
    }

    public class HMAC256Base64TokenHash : ITokenHashGenerator
    {
        private byte[] SecretHashKeyBytes { get; }

        public HMAC256Base64TokenHash(string hashKey) : this(Encoding.ASCII.GetBytes(hashKey)) { }

        public HMAC256Base64TokenHash(byte[] hashKey)
        {
            SecretHashKeyBytes = hashKey;
        }

        public string Hash(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            using var hmac = new HMACSHA256(SecretHashKeyBytes);
            var hashArray = hmac.ComputeHash(byteArray);
            // Standard Base64 encoding first
            string base64 = Convert.ToBase64String(hashArray);

            // Convert to Base64URL (JWT style)
            string base64Url = base64
                .TrimEnd('=')          // Remove padding
                .Replace('+', '-')     // URL-safe
                .Replace('/', '_');    // URL-safe

            return base64Url;
        }

        public bool ValidHash(string input)
        {
            return input.Length == 43;
        }
    }
}