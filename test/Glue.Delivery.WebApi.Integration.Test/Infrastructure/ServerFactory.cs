using System.Collections.Generic;
using Glue.Delivery.Services;
using Glue.Delivery.WebApi.Integration.Test.Services;
using Glue.Delivery.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Glue.Delivery.WebApi.Integration.Test.Infrastructure
{
    public class ServerFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder()
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authentication:Secret", "0D49215F-76DF-471F-9F07-5E1B18BE1150")
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
                services.ReplaceServiceType<IDeliveryStateService, TestDeliveryStateService>();
                services.RemoveAll(typeof(IHostedService));
            });
        }
    }
}