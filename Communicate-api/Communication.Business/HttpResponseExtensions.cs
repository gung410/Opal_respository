using Microsoft.AspNetCore.Http;

namespace Communication.Business
{
    public static class HttpResponseExtensions
    {
        public static bool IsSuccessStatusCode(this HttpResponse httpResponse)
        {
            return ((int)httpResponse.StatusCode >= 200) && ((int)httpResponse.StatusCode <= 299);
        }
    }
}