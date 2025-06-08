using WebToken.Crypto;
using WebToken.Hash;
using WebToken.Model;
using WebToken.Serializer;

namespace WebToken.Service
{
    public class CryptoWebTokenService : WebTokenService
    {
        private readonly IWebTokenCryptoProvider _cryptoProvider;

        public CryptoWebTokenService(IWebTokenSerializer serializer, ITokenHashGenerator hashGenerator, IWebTokenCryptoProvider cryptoProvider) : base(serializer, hashGenerator)
        {
            _cryptoProvider = cryptoProvider;
        }

        public override byte[] Serialize(ITokenContainerModel data) => _cryptoProvider.Encrypt(base.Serialize(data));

        public override T Deserialize<T>(byte[] input) => base.Deserialize<T>(_cryptoProvider.Decrypt(input));
    }
}