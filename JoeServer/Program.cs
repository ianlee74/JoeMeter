using System;
using System.Collections;
using System.Net;
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
using GTI = Gadgeteer.Interfaces;
using WebRequest = MicroWebServer.WebRequest;
using WebResponse = MicroWebServer.WebResponse;

namespace JoeServer
{
    public partial class Program
    {
        private static Thread _connectEthernetThread = null;
        private static readonly byte[] _macAddress = new byte[] { 0x00, 0x75, 0x65, 0xde, 0xac, 0xaa };
        private static NetworkInterface _ni = null;

        private GTI.PWMOutput _headServoPwm;
        private GTI.PWMOutput _rightArmServoPwm;
        private GTI.PWMOutput _leftArmServoPwm;
        private GTI.PWMOutput _rightLegServoPwm;
        private GTI.PWMOutput _leftLegServoPwm;

        public MovingBodyPart LeftArm { get; private set; }
        public MovingBodyPart RightArm { get; private set; }
        public MovingBodyPart LeftLeg { get; private set; }
        public MovingBodyPart RightLeg { get; private set; }
        public MovingBodyPart Head { get; private set; }

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            //Debug.EnableGCMessages(false);
            Debug.Print("Web Server test software");

            _headServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Eight);
            _rightArmServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Nine);
            _leftArmServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Seven);
            _rightLegServoPwm = lowerServos.SetupPWMOutput(GT.Socket.Pin.Eight);
            _leftLegServoPwm = lowerServos.SetupPWMOutput(GT.Socket.Pin.Seven);

            LeftArm = new MovingBodyPart(_leftArmServoPwm, 0, 115, 115);
            RightArm = new MovingBodyPart(_rightArmServoPwm, 0, 145, 0);
            LeftLeg = new MovingBodyPart(_leftLegServoPwm, 130, 180, 180);
            RightLeg = new MovingBodyPart(_rightLegServoPwm, 0, 60, 0);
            Head = new MovingBodyPart(_headServoPwm, 20, 160, 90);

            // Print the network interface information to the debug interface
            //_ni = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            //_ni.EnableDhcp();
            
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

            InitializeNetwork_Static();
            
            //Debug.Print("IP Address = " + _ni.IPAddress + ", Gateway = " + _ni.GatewayAddress + ", MAC = " + _ni.PhysicalAddress);

            var webServer = new WebServer(null, Resources.ResourceManager);
            //webServer.Add(new RequestRoute("/", Resources.StringResources.Index, "text/html"));
            //webServer.Add(new RequestRoute("/MyStyles.css", Resources.StringResources.MyStyles, "text/css"));
            webServer.Add(new RequestRoute("/test", HttpMethods.GET, request => new HtmlResponse("Hello World !")));
            webServer.Add(new RequestRoute("/redirect", HttpMethods.GET, request => new RedirectResponse("/")));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.GET, GetTime));
            webServer.Add(new RequestRoute("/api/time", HttpMethods.PUT, SetTime));
            webServer.Add(new RequestRoute("/api/leftLeg", HttpMethods.GET, MoveLeftLeg));
            webServer.Add(new RequestRoute("/api/rightLeg", HttpMethods.GET, MoveRightLeg));
            webServer.Add(new RequestRoute("/api/leftArm", HttpMethods.GET, MoveLeftArm));
            webServer.Add(new RequestRoute("/api/rightArm", HttpMethods.GET, MoveRightArm));
            webServer.Add(new RequestRoute("/api/head", HttpMethods.GET, MoveHead));
        }

        private void StopMoving()
        {
            
        }

        private WebResponse MoveLeftLeg(WebRequest request)
        {
            LeftLeg.StartExercising();
            return new JsonResponse("ok");
        }

        private WebResponse MoveRightLeg(WebRequest request)
        {
            RightLeg.StartExercising();
            return new JsonResponse("ok");
        }

        private WebResponse MoveLeftArm(WebRequest request)
        {
            LeftArm.StartExercising();
            return new JsonResponse("ok");
        }

        private WebResponse MoveRightArm(WebRequest request)
        {
            RightArm.StartExercising();
            return new JsonResponse("ok");
        }

        private WebResponse MoveHead(WebRequest request)
        {
            LeftLeg.StartExercising();
            return new JsonResponse("ok");
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
            string myIP = "192.168.201.254";
            string subnetMask = "255.255.252.0";
            string gatewayAddress = "192.168.200.2";
            string dnsAddresses = "208.67.220.220";

            Debug.Print("Initializing network...");
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (interfaces != null && interfaces.Length > 0)
            {
                _ni = interfaces[0];
                Boolean isDhcpWorked = false;

                Debug.Print("Setting static IP...");
                _ni.EnableStaticIP(myIP, subnetMask, gatewayAddress);
                _ni.PhysicalAddress = _macAddress;
                _ni.EnableStaticDns(new[] { dnsAddresses });

                Debug.Print("Network ready.");
                Debug.Print(" IP Address: " + _ni.IPAddress);
                Debug.Print(" Subnet Mask: " + _ni.SubnetMask);
                Debug.Print(" Default Gateway: " + _ni.GatewayAddress);
                Debug.Print(" DNS Server: " + _ni.DnsAddresses[0]);
            }
            else
            {
                Debug.Print("No network device found.");
            }
        }
    }
}
