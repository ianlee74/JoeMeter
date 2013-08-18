using System;
using System.Net;
using Microsoft.SPOT;

namespace MicroWebServer
{
    public class RedirectResponse: WebResponse
    {
        public string TargetAddress { get; private set; }

        public RedirectResponse(string targetAddress) : base(
            HttpStatusCode.MovedPermanently, null,null)
        {
            AddHeader("Location", targetAddress);
        }
    }
}
