using WebToken.Model;

namespace WebToken.Service
{
    public interface IWebTokenService
    {
        public string Encode(ITokenContainerModel data);
        public bool TryDecode<T>(string token, out T container) where T : ITokenContainerModel;
    }
}