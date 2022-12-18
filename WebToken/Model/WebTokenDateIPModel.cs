using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebToken.Model
{
    public class WebTokenDateIPModel : ITokenContainerModel
    {
        public WebTokenDateIPModel(){}
        public WebTokenDateIPModel(TimeSpan expirationSpan) : this(expirationSpan, default){}
        public WebTokenDateIPModel(TimeSpan expirationSpan, string ip)
        {
            Expiration = DateTimeOffset.UtcNow + expirationSpan;
            Ip = ip;
        }
        public WebTokenDateIPModel(DateTimeOffset expiration, string ip)
        {
            Expiration = expiration;
            Ip = ip;
        }
        public WebTokenDateIPModel(string ip)
        {
            Ip = ip;
        }

        [JsonProperty("Exp")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset Expiration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ip { get; set; }

        public virtual bool IsValid(ITokenContainerModel validationParams)
        {
            if (validationParams is not WebTokenDateIPModel vParams) return false;
            if (DateTimeOffset.UtcNow > Expiration) return false;
            return Ip == vParams.Ip;
        }
    }
}