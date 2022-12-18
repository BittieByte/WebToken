using System.Linq;
using System.Text;
using WebToken.Crypto;
using WebToken.Model;
using WebToken.Serializer;

namespace WebToken.Service
{
    public class CryptoWebTokenService : WebTokenService
    {
        private readonly IWebTokenCryptoProvider _cryptoProvider;

        public CryptoWebTokenService(string hashKey, IWebTokenSerializer serializer,
            IWebTokenCryptoProvider cryptoProvider) : base(
            string.Join(string.Empty, hashKey, Encoding.UTF8.GetString(cryptoProvider.GetBytes())),
            serializer)
        {
            _cryptoProvider = cryptoProvider;
        }

        public CryptoWebTokenService(byte[] hashKey,
            IWebTokenSerializer serializer, IWebTokenCryptoProvider cryptoProvider) : base(
            hashKey.Concat(cryptoProvider.GetBytes()).ToArray(),
            serializer)
        {
            _cryptoProvider = cryptoProvider;
        }

        public override byte[] Serialize(ITokenContainerModel data) => _cryptoProvider.Encrypt(base.Serialize(data));

        public override T Deserialize<T>(byte[] input) => base.Deserialize<T>(_cryptoProvider.Decrypt(input));
    }
}