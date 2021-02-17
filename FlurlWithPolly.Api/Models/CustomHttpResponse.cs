using System.Net;

namespace FlurlWithPolly.Api.Models
{
    public class CustomHttpResponse<T>
    {
        public CustomHttpResponse(HttpStatusCode httpStatusCode, T response)
        {
            HttpStatusCode = httpStatusCode;
            Response = response;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
        public T Response { get; set; }
    }
}
