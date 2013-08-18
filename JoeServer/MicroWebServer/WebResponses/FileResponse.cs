using System.Net;

namespace MicroWebServer
{
    public class FileResponse: WebResponse
    {
        public FileResponse(string content, string contentType) : base(HttpStatusCode.OK, content, contentType)
        {
        }
    }
}
