using System.IO;
using System.Net;

namespace MicroWebServer
{
    public abstract class WebResponse
    {
        public HttpStatusCode StatusCode { get; protected set; }
        public string Content { get; protected set; }
        public string ContentType { get; protected set; }
        private readonly WebHeaderCollection additionalHeaders;

        protected WebResponse(HttpStatusCode statusCode, string content, string contentType)
        {
            StatusCode = statusCode;
            Content = content;
            ContentType = contentType;
            additionalHeaders = new WebHeaderCollection();
        }

        protected void AddHeader(string key, string value)
        {
            additionalHeaders.Add(key, value);
        }

        public void Respond(HttpListenerContext listenerContext)
        {
            var response = listenerContext.Response;
            response.ContentType = ContentType;
            response.StatusCode = (int)StatusCode;
            foreach (string headerKey in additionalHeaders.AllKeys)
                listenerContext.Response.Headers.Add(headerKey, additionalHeaders[headerKey]);
            Stream output = response.OutputStream;
            
            if (Content != null)
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Content);
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                //TODO: testen wat er gebeurt als er geen content is !!
            }
            output.Close();
        }
    }
}
