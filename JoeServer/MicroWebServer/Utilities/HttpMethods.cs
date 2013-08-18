using System;
using Microsoft.SPOT;

namespace MicroWebServer
{
    public enum HttpMethods
    {
        GET, 
        POST, 
        PUT, 
        DELETE
    }

    public static class HttpMethodParser
    {
        public static HttpMethods HttpMethodParse(this string method)
        {
            switch(method)
            {
                case "GET": return HttpMethods.GET;
                case "POST": return HttpMethods.POST;
                case "DELETE": return HttpMethods.DELETE;
                case "PUT": return HttpMethods.PUT;
                default: throw new ApplicationException("Unknown Http Method");
            }
        }
    }
}
