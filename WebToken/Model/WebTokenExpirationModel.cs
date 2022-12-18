using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebToken.Model
{
    public class WebTokenExpirationModel : ITokenContainerModel
    {
        public WebTokenExpirationModel() { }
        public WebTokenExpirationModel(TimeSpan expirationSpan)
        {
            Expiration = DateTimeOffset.UtcNow + expirationSpan;
        }

        public WebTokenExpirationModel(DateTimeOffset expiration)
        {
            Expiration = expiration;
        }

        [JsonProperty("Exp", ItemConverterType = typeof(UnixDateTimeConverter))]
        public DateTimeOffset Expiration { get; set; }

        public virtual bool IsValid(ITokenContainerModel validationParams)
        {
            return DateTimeOffset.UtcNow <= Expiration;
        }
    }
}