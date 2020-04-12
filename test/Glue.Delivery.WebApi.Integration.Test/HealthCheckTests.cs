using System.Net.Http;
using Glue.Delivery.WebApi.Integration.Test.Infrastructure;
using Xunit;

namespace Glue.Delivery.WebApi.Integration.Test
{
    public class HealthCheckTests : IClassFixture<ServerFactory>
    {
        private readonly HttpClient _client;
        
        public HealthCheckTests(ServerFactory serverFactory)
        {
            _client = serverFactory.CreateClient();
        }

        [Fact]
        public async void WebApi_Should_Respond_200_To_Ping_For_Health_Check()
        {
            var response = await _client.GetAsync("/ping");
            
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}