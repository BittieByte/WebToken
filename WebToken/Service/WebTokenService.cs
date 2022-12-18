using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WebToken.Model;
using WebToken.Serializer;

namespace WebToken.Service
{
    public class WebTokenService : IWebTokenService
    {
        private readonly IWebTokenSerializer _serializer;
        private byte[] secretHashKeyBytes { get; }

        public string Delimiter = "~";

        public WebTokenService(string hashKey, IWebTokenSerializer serializer) : this(Encoding.ASCII.GetBytes(hashKey), serializer) { }

        public WebTokenService(byte[] hashKey, IWebTokenSerializer serializer)
        {
            _serializer = serializer;
            secretHashKeyBytes = hashKey;
        }

        public virtual byte[] Serialize(ITokenContainerModel data) => _serializer.Serialize(data);

        public virtual T Deserialize<T>(byte[] input) => _serializer.Deserialize<T>(input);

        public string GenerateToken(ITokenContainerModel data)
        {
            var payload = Convert.ToBase64String(Serialize(data)).TrimEnd('=');
            var hmac = CalculateHmac(payload, secretHashKeyBytes);
            return string.Join(Delimiter, payload, hmac);
        }

        public bool IsTokenValid<T>(string token, ITokenContainerModel validationParams, out T container) where T : ITokenContainerModel
        {
            container = default;
            if (string.IsNullOrWhiteSpace(token)) return false;
            var split = token.Split(Delimiter, 2);
            if (split.Length != 2) return false;
            if (split[1].Length != 64) return false;
            if (CalculateHmac(split[0], secretHashKeyBytes) != split[1]) return false;
            container = Deserialize<T>(Convert.FromBase64String(split[0] + GetPadding(split[0].Length)));
            return container.IsValid(validationParams);
        }

        public static string CalculateHmac(string input, byte[] key)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            using var hmac = new HMACSHA256(key);
            var hashArray = hmac.ComputeHash(byteArray);
            return hashArray.Aggregate("", (s, e) => s + $"{e:x2}", s => s);
        }

        private static string GetPadding(int length)
        {
            return (length % 4) switch
            {
                2 => "==",
                3 => "=",
                _ => string.Empty
            };
        }
    }
}