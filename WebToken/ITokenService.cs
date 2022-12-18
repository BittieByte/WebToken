using WebToken.Model;

namespace WebToken
{
    public interface IWebTokenService
    {
        public string GenerateToken(ITokenContainerModel data);
        public bool IsTokenValid<T>(string token, ITokenContainerModel validationParams, out T container) where T : ITokenContainerModel;
    }
}