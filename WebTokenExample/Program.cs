using System.Security.Cryptography;
using WebToken.Crypto;
using WebToken.Hash;
using WebToken.Model;
using WebToken.Serializer;
using WebToken.Service;
using WebToken.Validation;

namespace WebTokenExample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //create token service
            using Aes aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
            var tokenHashSalt = @"long salt here"; //static salt in safe storage
            var aesEncryptionKey = Convert.ToBase64String(aes.Key); //static key in safe storage
            var aesEncryptionIV = Convert.ToBase64String(aes.IV); //static IV in safe storage

            IWebTokenService tokenService = new CryptoWebTokenService(
                new JsonWebTokenSerializer(),
                new HMAC256Base64TokenHash(tokenHashSalt),
                new AesWebTokenCryptoProvider(aesEncryptionKey, aesEncryptionIV));


            //create token
            ITokenContainerModel model = new WebTokenBuilder<WebTokenModel>()
                .WithExpiration(TimeSpan.FromSeconds(5))
                .WithClaim("ip", "127.0.0.1")
                .WithClaim("id", "test")
                .Build();

            var token = tokenService.Encode(model);
            Console.WriteLine($"Token: {token}");

            //validate token when user supplies token
            Console.WriteLine(WebTokenValidator.IsValid<WebTokenModel>(tokenService, token, out var tokenObject, ("ip", "127.0.0.1"), ("id", WebTokenValidator.SkipValueCheckObject))); // Valid
            if (tokenObject.TryGetClaim("id", out string id)) Console.WriteLine($"Id: {id}");
            Console.WriteLine(WebTokenValidator.IsValid<WebTokenModel>(tokenService, token, ("ip", "127.0.0.2"))); // Invalid Ip
            await Task.Delay(6000);//Wait 6 seconds
            Console.WriteLine(WebTokenValidator.IsValid<WebTokenModel>(tokenService, token, ("ip", "127.0.0.1"))); // Expired
        }
    }
}