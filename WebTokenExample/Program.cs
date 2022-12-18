using System.Security.Cryptography;
using WebToken;
using WebToken.Crypto;
using WebToken.Model;
using WebToken.Serializer;
using WebToken.Service;

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
                tokenHashSalt,
                new JsonWebTokenSerializer(),
                new AesWebTokenCryptoProvider(aesEncryptionKey, aesEncryptionIV));

            //create token
            ITokenContainerModel model = new WebTokenDateIPModel(TimeSpan.FromSeconds(5), "127.0.0.1");
            var token = tokenService.GenerateToken(model);
            Console.WriteLine($"Token: {token}");


            //validate token when user supplies token
            Valid(tokenService, new WebTokenDateIPModel(), token, "127.0.0.1"); // Valid
            Valid(tokenService, new WebTokenDateIPModel(), token, "127.0.0.2"); // Invalid IP
            await Task.Delay(6000);//Wait 6 seconds
            Valid(tokenService, new WebTokenDateIPModel(), token, "127.0.0.1"); // Expired
        }

        public static void Valid(IWebTokenService tokenService, ITokenContainerModel validationParams, string token, string currentIp)
        {
            if (!tokenService.IsTokenValid<WebTokenDateIPModel>(token, validationParams, out var result))
            {
                //expire if invalid 

                if ((result != default))
                {
                    if (result.Ip != null && result.Ip != currentIp)
                    {
                        //InvalidIP ~ Token was generated for a different ip
                        Console.WriteLine($"InvalidIP");
                        return;
                    }
                    //Token expired
                    Console.WriteLine("Expired");
                    return;
                }
                //Token Malformed / Attempted tampering
                Console.WriteLine("Malformed");
                return;
            }
            else
            {
                //Valid Token
                Console.WriteLine("Valid");
            }
        }
    }
}