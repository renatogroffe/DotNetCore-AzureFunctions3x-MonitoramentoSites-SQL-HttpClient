using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ServerlessMonitorSitesSQL.Clients;

[assembly: FunctionsStartup(typeof(ServerlessMonitorSitesSQL.Startup))]
namespace ServerlessMonitorSitesSQL
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<TesteSiteClient>();
        }
    }
}