using System.Collections.Generic;
using Glue.Delivery.Services;
using Glue.Delivery.WebApi.Integration.Test.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Glue.Delivery.WebApi.Integration.Test.Infrastructure
{
    public class ServerFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder()
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                }));
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Config for our new services
            builder.UseConfiguration(
                new ConfigurationBuilder().AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                }).Build());

            builder.ConfigureServices(services =>
            {
                //Service replacement
                services.ReplaceServiceType<IDeliveryService, TestDeliveryService>();
            });
        }
    }
}