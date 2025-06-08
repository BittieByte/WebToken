using System.Collections.Generic;

namespace WebToken.Model
{
    public interface ITokenContainerModel
    {
        public IDictionary<string, object> Claims { get; set; }
    }
}