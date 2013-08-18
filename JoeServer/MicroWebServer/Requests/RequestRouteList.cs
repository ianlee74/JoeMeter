using System.Collections;

namespace MicroWebServer
{
    public class RequestRouteList : IEnumerable
    {
        private readonly Hashtable table;

        public RequestRouteList()
        {
            table = new Hashtable(25);
        }

        public IEnumerator GetEnumerator()
        {
            return table.GetEnumerator();
        }

        public void Add(RequestRoute route)
        {
            //TODO: nog controleren of het pad de juiste vorm heeft (begint met /, bevat alleen /, letters en cijfers, ...)
            if (route.IsFileResponse)
                table.Add(HttpMethods.GET.ToString() + "_" + route.Path, route);
            else
                table.Add(route.HttpMethod.ToString() + "_" + route.Path, route);
        }

        public bool Contains(HttpMethods httpMethod, string path)
        {
            return table.Contains(httpMethod.ToString() + "_" + path);
        }

        public RequestRoute Find(HttpMethods httpMethod, string path)
        {
            return (RequestRoute)table[httpMethod.ToString() + "_" + path];
        }
    }
}
