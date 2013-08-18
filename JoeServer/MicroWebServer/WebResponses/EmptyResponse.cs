using System.Net;

namespace MicroWebServer
{
    public class EmptyResponse : WebResponse
    {
        public EmptyResponse() : base(HttpStatusCode.OK, null, "none")
        {
        }
    }
}
