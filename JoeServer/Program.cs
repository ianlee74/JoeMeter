using System;
using System.Collections;
using System.Threading;
using MicroWebServer;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace JoeServer
{
    public partial class Program
    {
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            //Debug.EnableGCMessages(false);
            Debug.Print("Web Server test software");

            // Print the network interface information to the debug interface
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface NI = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            //NI.EnableDhcp();
            Debug.Print("IP Address = " + NI.IPAddress + ", Gateway = " + NI.GatewayAddress + ", MAC = " + NI.PhysicalAddress);

            var webServer = new WebServer(null, Resources.ResourceManager);
            webServer.Add(new RequestRoute("/", Resources.StringResources.Index, "text/html"));
            webServer.Add(new RequestRoute("/MyStyles.css", Resources.StringResources.MyStyles, "text/css"));
            webServer.Add(new RequestRoute("/test", HttpMethods.GET, request => new HtmlResponse("Hello World !")));
            webServer.Add(new RequestRoute("/redirect", HttpMethods.GET, request => new RedirectResponse("/")));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.GET, GetTime));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.PUT, SetTime));
            
        }
    }
}
