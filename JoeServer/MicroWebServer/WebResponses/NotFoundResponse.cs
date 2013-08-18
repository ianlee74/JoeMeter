using System.Net;

namespace MicroWebServer
{
    public class NotFoundResponse : WebResponse
    {
        public NotFoundResponse(string requestedUri) :
            base(
                HttpStatusCode.NotFound,
                "<html><body><p>'" + requestedUri + "' is not available on this server</p></body></html>",
                "text/html"
            )
        {
        }
    }
}
