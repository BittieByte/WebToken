# WebToken
 A C# library designed for creation of secure web tokens

Example usage
```cs
//create token service
var tokenHashSalt = @"long salt here"; //static salt in safe storage
var aesEncryptionKey = Convert.ToBase64String(aes.Key); //static key in safe storage
var aesEncryptionIV = Convert.ToBase64String(aes.IV); //static IV in safe storage

IWebTokenService tokenService = new CryptoWebTokenService(
    tokenHashSalt,
    new JsonWebTokenSerializer(),
    new AesWebTokenCryptoProvider(aesEncryptionKey, aesEncryptionIV));

//create token and supply to user
ITokenContainerModel model = new WebTokenDateIPModel(TimeSpan.FromMinutes(30), "127.0.0.1");
var token = tokenService.GenerateToken(model);
Console.WriteLine($"Token: {token}");

//validate token when user supplies token
if (!tokenService.IsTokenValid<WebTokenDateIPModel>(token, new WebTokenDateIPModel(), out var result))
{
	//Invalid
}
else
{
	//Valid Token
}
```
