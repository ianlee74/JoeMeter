using System;
using System.Net;
using System.Threading;
using Microsoft.SPOT;
using System.Collections;

namespace MicroWebServer
{
    public class WebRequest
    {
        public string Uri { get; private set; }
        public string ContentType { get; private set; }
        public string Content { get; private set; }
        public Hashtable PostValues { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
        public HttpMethods Method { get; private set; }

        public WebRequest(HttpListenerRequest listenerRequest)
        {
            Uri = listenerRequest.Url.OriginalString;
            ContentType = listenerRequest.ContentType;
            Headers = listenerRequest.Headers;
            //Method = listenerRequest.HttpMethod;

            PostValues = new Hashtable(10);
            var stream = listenerRequest.InputStream;
            int length = (int) listenerRequest.ContentLength64;
            //Debug.Print("Initial Length = " + length);
            //if (length == 0)
            //{
            //    Thread.Sleep(300); //1000ms OK, 500ms OK, 200ms NOK, 300ms OK, 250 NOK, 275ms OK, 260ms NOK
            //    length = (int)stream.Length;
            //    Debug.Print("After wait Length = " + length);
            //}

            if (length > 0)
            {
                var buffer = new byte[256];
                int counter = 0;
                while(stream.Length < length) // This while loop must be here to avoid mishaps !!
                {
                    Thread.Sleep(10); 
                    counter++;
                }
                Debug.Print("Waited for " + (counter*10) + "ms before the stream was ready");
                //stream.ReadTimeout = 1000;
                stream.Read(buffer, 0, length);
                Content = StringUtils.ByteArrayToString(buffer, 0, length);
                //string decoded = HttpUtility.UrlDecode();
                //foreach (string part in decoded.Split('&'))
                //{
                //    string[] subparts = part.Split('=');
                //    if (!PostValues.Contains(subparts[0]))
                //        PostValues.Add(subparts[0], subparts[1]);
                //}
            }
        }
    }
}
