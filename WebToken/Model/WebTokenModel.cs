using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebToken.Model
{
    public class WebTokenModel : ITokenContainerModel
    {
        public WebTokenModel() { }

        [JsonProperty(nameof(Claims))]
        public IDictionary<string, object> Claims { get; set; } = new Dictionary<string, object>();
    }
}