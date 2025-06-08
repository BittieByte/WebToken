using System;
using System.Text;
using WebToken.Hash;
using WebToken.Model;
using WebToken.Serializer;

namespace WebToken.Service
{

    public class WebTokenService : IWebTokenService
    {
        private readonly IWebTokenSerializer _serializer;
        private readonly ITokenHashGenerator _hashGenerator;

        public string Delimiter = ".";


        public WebTokenService(IWebTokenSerializer serializer, ITokenHashGenerator hashGenerator)
        {
            _serializer = serializer;
            _hashGenerator = hashGenerator;
        }

        public virtual byte[] Serialize(ITokenContainerModel data) => _serializer.Serialize(data);

        public virtual T Deserialize<T>(byte[] input) => _serializer.Deserialize<T>(input);

        public string Encode(ITokenContainerModel data)
        {
            var payload = Convert.ToBase64String(Serialize(data)).TrimEnd('=');
            return string.Join(Delimiter, payload, _hashGenerator.Hash(payload));
        }

        public bool TryDecode<T>(string token, out T container) where T : ITokenContainerModel
        {
            container = default;
            if (string.IsNullOrWhiteSpace(token)) return false; //input is empty
            var split = token.Split(Delimiter, 2); //split token
            if (split.Length != 2) return false; // invalid token parts
            if (!_hashGenerator.ValidHash(split[1])) return false; //if implemented checks if hash is valid
            if (_hashGenerator.Hash(split[0]) != split[1]) return false; //hashes data and checks against hash
            container = Deserialize<T>(Convert.FromBase64String(split[0] + GetPadding(split[0].Length)));// valid data - safe to deserialize
            return true;
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