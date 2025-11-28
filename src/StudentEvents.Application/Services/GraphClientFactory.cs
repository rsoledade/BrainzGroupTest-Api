using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace StudentEvents.Application.Services
{
    public class GraphClientFactory : IGraphClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GraphClientFactory(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient Create()
        {
            var clientId = _configuration.GetValue<string>("Graph:ClientId");
            var clientSecret = _configuration.GetValue<string>("Graph:ClientSecret");
            var tenantId = _configuration.GetValue<string>("Graph:TenantId");

            if (string.IsNullOrEmpty(clientId))
                throw new InvalidOperationException("Missing configuration 'Graph:ClientId'. Set it via 'dotnet user-secrets set \"Graph:ClientId\" \"<value>\"' (run in the API project) or an environment variable.");
            if (string.IsNullOrEmpty(clientSecret))
                throw new InvalidOperationException("Missing configuration 'Graph:ClientSecret'. Set it via 'dotnet user-secrets set \"Graph:ClientSecret\" \"<value>\"' (run in the API project) or an environment variable.");
            if (string.IsNullOrEmpty(tenantId))
                throw new InvalidOperationException("Missing configuration 'Graph:TenantId'. Set it via 'dotnet user-secrets set \"Graph:TenantId\" \"<value>\"' (run in the API project) or an environment variable.");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var result = cca.AcquireTokenForClient(scopes).ExecuteAsync().GetAwaiter().GetResult();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://graph.microsoft.com/v1.0/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return client;
        }
    }
}
