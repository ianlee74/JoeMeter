using System;

namespace MicroWebServer
{
    public class RequestRoute
    {
        public string Path { get; private set; }
        public WebRequestHandler RequestHandler { get; private set; }
        public HttpMethods HttpMethod { get; set; }
        public Enum FileResource { get; private set; }
        public string ContentType { get; private set; }
        public bool IsFileResponse { get { return RequestHandler == null; } }
        public bool IsCallbackResponse { get { return RequestHandler != null; } }

        public RequestRoute(string path, HttpMethods httpMethod, WebRequestHandler requestHandler)
        {
            Path = path;
            HttpMethod = httpMethod;
            RequestHandler = requestHandler;
        }

        public RequestRoute(string path, Enum fileResource, string contentType)
        {
            Path = path;
            FileResource = fileResource;
            ContentType = contentType;
        }
    }
}
