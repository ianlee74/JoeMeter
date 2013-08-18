namespace MicroWebServer
{
    public class JsonResponse : FileResponse
    {
        public JsonResponse(string jsonContent)
            : base(jsonContent, "application/json")
        {}
    }
}
