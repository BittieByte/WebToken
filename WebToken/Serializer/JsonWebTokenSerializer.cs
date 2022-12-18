using System.Text;
using Newtonsoft.Json;
using WebToken.Model;

namespace WebToken.Serializer
{
    public class JsonWebTokenSerializer : IWebTokenSerializer
    {
        public Encoding StringEncoding = Encoding.ASCII;

        public byte[] Serialize(ITokenContainerModel data) => StringEncoding.GetBytes(JsonConvert.SerializeObject(data));

        public T Deserialize<T>(byte[] input) => JsonConvert.DeserializeObject<T>(StringEncoding.GetString(input));
    }
}