using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FlurlWithPolly.Api.PollyPolicies
{
    public class PolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return PolicyUtil.PolicyStrategy
                .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
