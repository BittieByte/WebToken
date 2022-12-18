using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using WebToken.Model;

namespace WebToken.Serializer
{
    public class XmlWebTokenSerializer : IWebTokenSerializer
    {
        public Encoding StringEncoding = Encoding.ASCII;

        public byte[] Serialize(ITokenContainerModel data) => StringEncoding.GetBytes(XmlSerializeToString(data));

        public T Deserialize<T>(byte[] input) => XmlDeserializeFromString<T>(StringEncoding.GetString(input));

        private static string XmlSerializeToString(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();
            using TextWriter writer = new StringWriter(sb);
            serializer.Serialize(writer, objectInstance);
            return sb.ToString();
        }

        private static T XmlDeserializeFromString<T>(string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        private static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            using TextReader reader = new StringReader(objectData);
            var result = serializer.Deserialize(reader);
            return result;
        }
    }
}