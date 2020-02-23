using System.Net.Http;
using System.Net.Http.Headers;

namespace ServerlessMonitorSitesSQL.Clients
{
    public class TesteSiteClient
    {
        private HttpClient _client;
        
        public TesteSiteClient(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpResponseMessage EnviarRequisicaoTeste(string urlSite)
        {
            return _client.GetAsync(urlSite).Result;
        }
    }
}