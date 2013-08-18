using System;
using System.Collections;
using System.Threading;
using MicroWebServer;
using MicroWebServer.Json;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
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
        private static Thread _connectEthernetThread = null;
        private static readonly byte[] _macAddress = new byte[] { 0x00, 0x75, 0x65, 0xde, 0xac, 0xaa };
        private static string _ipAddress = "0.0.0.0";

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            //Debug.EnableGCMessages(false);
            Debug.Print("Web Server test software");

            // Print the network interface information to the debug interface
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface NI = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            //NI.EnableDhcp();
            Microsoft.SPOT.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += (sender, args) =>
            {
#if DEBUG
                Debug.Print("Network availability changed!  " + args.IsAvailable);
#endif
                if (args.IsAvailable && !_connectEthernetThread.IsAlive)
                {
                    // Initialize the network
                    _connectEthernetThread = new Thread(InitializeNetwork_Static);
                    _connectEthernetThread.Start();
                    Debug.Print("Network intialized.");
                }
                else
                {
                    Debug.Print("Huh?");
                }
            };
            Debug.Print("IP Address = " + NI.IPAddress + ", Gateway = " + NI.GatewayAddress + ", MAC = " + NI.PhysicalAddress);

            var webServer = new WebServer(null, Resources.ResourceManager);
            //webServer.Add(new RequestRoute("/", Resources.StringResources.Index, "text/html"));
            //webServer.Add(new RequestRoute("/MyStyles.css", Resources.StringResources.MyStyles, "text/css"));
            webServer.Add(new RequestRoute("/test", HttpMethods.GET, request => new HtmlResponse("Hello World !")));
            webServer.Add(new RequestRoute("/redirect", HttpMethods.GET, request => new RedirectResponse("/")));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.GET, GetTime));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.PUT, SetTime));
        }

        private static WebResponse GetTime(WebRequest request)
        {
            var now = DateTime.Now;
            Debug.Print("Sending " + now.ToLocalTime());
            return new JsonResponse("\"" + JsonDateTime.ToASPNetAjax(now) + "\"");
        }

        private static WebResponse SetTime(WebRequest request)
        {
            var time = JsonDateTime.FromASPNetAjax(request.Content);
            Utility.SetLocalTime(time);
            Debug.Print("Time Set to " + time.ToUniversalTime() + " (day = " + time.Day + ")");
            return new EmptyResponse();
        }

        private static void InitializeNetwork_Static()
        {
            string myIP = "192.168.1.99";
            string subnetMask = "255.255.255.0";
            string gatewayAddress = "192.168.1.1";
            string dnsAddresses = "192.168.1.1";

            Debug.Print("Initializing network...");
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (interfaces != null && interfaces.Length > 0)
            {
                NetworkInterface networkInterface = interfaces[0];
                Boolean isDhcpWorked = false;

                Debug.Print("Setting static IP...");
                networkInterface.EnableStaticIP(myIP, subnetMask, gatewayAddress);
                networkInterface.PhysicalAddress = _macAddress;
                networkInterface.EnableStaticDns(new[] { dnsAddresses });

                Debug.Print("Network ready.");
                Debug.Print(" IP Address: " + networkInterface.IPAddress);
                Debug.Print(" Subnet Mask: " + networkInterface.SubnetMask);
                Debug.Print(" Default Gateway: " + networkInterface.GatewayAddress);
                Debug.Print(" DNS Server: " + networkInterface.DnsAddresses[0]);
            }
            else
            {
                Debug.Print("No network device found.");
            }
        }
    }
}
