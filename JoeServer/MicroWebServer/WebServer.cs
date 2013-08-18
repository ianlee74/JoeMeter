using System;
using System.Net;
using System.Threading;
using Microsoft.SPOT;
using System.Resources;

namespace MicroWebServer
{
    public class WebServer
    {
        private readonly RequestRouteList routeList;
        private Thread mainThread;
        private readonly ILog log;
        private HttpListener listener;
        private readonly ResourceManager resourceManager;

        public WebServer(ILog log, ResourceManager resourceManager)
        {
            this.log = log;
            routeList = new RequestRouteList();
            mainThread = new Thread(MainThreadHandler);
            mainThread.Start();
            this.resourceManager = resourceManager;
        }

        public bool MainThreadIsAlive { get { return mainThread.IsAlive; } }

        public ThreadState MainThreadStatus { get { return mainThread.ThreadState; } }

        public void RestartMainThread()
        {
            mainThread.Abort();
            Thread.Sleep(2000);
            //log.Report(PriorityType.Alert, "Webserver", "Creating a new main thread for the webserver !!!");
            mainThread = new Thread(MainThreadHandler);
            mainThread.Start();
        }

        public void Add(RequestRoute route)
        {
            routeList.Add(route);
        }

        private void MainThreadHandler()
        {
            listener = new HttpListener("http");
            bool abortRequested = false;
            listener.Start();

            //TODO: Threading verhaal nog te bekijken !!
            while (!abortRequested)
            {
                try
                {
                    Debug.Print("Webserver while loop beginning");
                    var context = listener.GetContext();
                    var path = context.Request.Url.OriginalString;
                    var method = context.Request.HttpMethod.HttpMethodParse();
                    //Debug.Print("Webserver incoming request : '" + url + "'");

                    WebResponse response = new NotFoundResponse(path);

                    if (routeList.Contains(method, path))
                    {
                        var route = routeList.Find(method, path);
                        if (route.IsFileResponse)
                        {
                            string content = (string) ResourceUtility.GetObject(resourceManager, route.FileResource);
                            response = new FileResponse(content, route.ContentType);
                        }
                        else
                        {
                            var webRequest = new WebRequest(context.Request);
                            response = route.RequestHandler(webRequest);
                        }
                    }

                    response.Respond(context);
                    context.Close();
                }
                catch (Exception exception)
                {
                    listener.Stop();
                    listener.Close();
                    abortRequested = true;
                }
            }
        }
    }
}