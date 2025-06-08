using System;
using System.Linq;
using System.Text;
using WebToken.Model;

namespace WebToken.Serializer
{
    public class CsvWebTokenSerializer : IWebTokenSerializer
    {
        public Encoding StringEncoding = Encoding.ASCII;

        public byte[] Serialize(ITokenContainerModel data)
        {
            var csvString = string.Join(",",
                data.Claims.Select(kvp =>
                    $"{Escape(kvp.Key)}={Escape(kvp.Value?.ToString() ?? string.Empty)}"
                )
            );

            return StringEncoding.GetBytes(csvString);
        }

        public T Deserialize<T>(byte[] input)
        {
            var csvString = StringEncoding.GetString(input);

            // If T implements ITokenContainerModel, populate it
            if (typeof(ITokenContainerModel).IsAssignableFrom(typeof(T)))
            {
                var token = Activator.CreateInstance<T>();

                if (token is ITokenContainerModel container)
                {
                    if (!string.IsNullOrWhiteSpace(csvString))
                    {
                        var pairs = csvString.Split(',');

                        foreach (var pair in pairs)
                        {
                            var kv = pair.Split(new[] { '=' }, 2);
                            if (kv.Length == 2)
                            {
                                var key = Unescape(kv[0]);
                                var value = Unescape(kv[1]);
                                container.Claims[key] = value;
                            }
                        }
                    }

                    return token;
                }
            }

            // Fallback: if T is not ITokenContainerModel, throw or return default
            throw new InvalidOperationException($"Cannot deserialize CSV to type {typeof(T).Name}. It must implement ITokenContainerModel.");
        }

        // Basic escape for commas, equals, quotes
        private static string Escape(string input)
        {
            if (input.Contains(",") || input.Contains("=") || input.Contains("\""))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }

        private static string Unescape(string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                var unquoted = input[1..^1];
                return unquoted.Replace("\"\"", "\"");
            }
            return input;
        }
    }

}