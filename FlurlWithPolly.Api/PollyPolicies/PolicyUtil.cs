using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlurlWithPolly.Api.PollyPolicies
{
    public static class PolicyUtil
    {
        private const int TimeoutSeconds = 2;
        private const int RetryCount = 3;

        private static AsyncTimeoutPolicy<HttpResponseMessage> TimeoutPolicy
        {
            get
            {
                return Policy.TimeoutAsync<HttpResponseMessage>(TimeoutSeconds,
                    (context, timeSpan, task) =>
                    {
                        Debug.WriteLine($"[App|Policy]: Timeout delegate fired after {timeSpan.TotalSeconds:n1}s.");
                        return Task.CompletedTask;
                    });
            }
        }

        private static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(RetryCount,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (response, time) =>
                        {
                            Debug.WriteLine($"[App|Policy]: Retry delegate fired after {time.TotalSeconds:n1}s ({response?.Result?.StatusCode}).");
                        });
            }
        }

        public static AsyncPolicyWrap<HttpResponseMessage> PolicyStrategy => Policy.WrapAsync(RetryPolicy, TimeoutPolicy);
    }
}
