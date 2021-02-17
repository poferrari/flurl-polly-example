using Flurl.Http;
using Flurl.Http.Configuration;
using FlurlWithPolly.Api.Models;
using FlurlWithPolly.Api.PollyPolicies;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlurlWithPolly.Api.Acls
{
    public class HttpProvider
    {
        private static bool _flurlConfigured;

        public HttpProvider()
        {
            ConfigureFlurl();
        }

        public async Task PostAsync(string requestUri, object content, string token = null)
            => await BuilderPostAsync(requestUri, content, token);

        public async Task<CustomHttpResponse<TResponse>> PostAsync<TResponse>(string requestUri, object content, string token = null)
        {
            var response = await BuilderPostAsync(requestUri, content, token);
            return await CreateCustomHttpResponse<TResponse>(response);
        }

        public async Task<CustomHttpResponse<TResponse>> GetAsync<TResponse>(string requestUri, string token = null)
        {
            var response = await BuilderGetAsync(requestUri, token);
            return await CreateCustomHttpResponse<TResponse>(response);
        }

        private static void ConfigureFlurl()
        {
            if (_flurlConfigured)
            {
                return;
            }

            FlurlHttp.Configure(settings =>
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
                settings.HttpClientFactory = new PollyHttpClientFactory();
            });

            _flurlConfigured = true;
        }

        private async Task<IFlurlResponse> BuilderPostAsync(string requestUri, object content, string token)
            => await (token is null ?
                    requestUri.PostJsonAsync(content) :
                    requestUri.WithOAuthBearerToken(token)
                              .PostJsonAsync(content)
            );

        private async Task<IFlurlResponse> BuilderGetAsync(string requestUri, string token)
            => await (token is null ?
                    requestUri.GetAsync() :
                    requestUri.WithOAuthBearerToken(token)
                              .GetAsync()
            );

        private async Task<CustomHttpResponse<TResponse>> CreateCustomHttpResponse<TResponse>(IFlurlResponse response)
        {
            var statusCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), response.StatusCode);
            var valueResponse = await response.GetJsonAsync<TResponse>();

            return new CustomHttpResponse<TResponse>(statusCode, valueResponse);
        }
    }
}
