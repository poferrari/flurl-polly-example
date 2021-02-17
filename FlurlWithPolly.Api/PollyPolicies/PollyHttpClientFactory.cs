using Flurl.Http.Configuration;
using System.Net.Http;

namespace FlurlWithPolly.Api.PollyPolicies
{
    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }
}
