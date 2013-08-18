namespace MicroWebServer
{
    public class HtmlResponse : FileResponse
    {
        public HtmlResponse(string htmlContent) : base(htmlContent, "text/html")
        {}
    }
}
