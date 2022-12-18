using WebToken.Model;

namespace WebToken.Serializer
{
    public interface IWebTokenSerializer
    {
        public byte[] Serialize(ITokenContainerModel data);
        public T Deserialize<T>(byte[] input);
    }
}