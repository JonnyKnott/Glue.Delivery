using System.Net.Http;
using System.Text;
using Glue.Delivery.Models.ApiModels.Delivery;
using Glue.Delivery.WebApi.Controllers;
using Glue.Delivery.WebApi.Integration.Test.Infrastructure;
using Glue.Delivery.WebApi.Integration.Test.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Glue.Delivery.WebApi.Integration.Test.Controllers
{
    public class DeliveryControllerTests : IClassFixture<ServerFactory>
    {
        private readonly HttpClient _client;

        private const string Endpoint = "api/v1/delivery";
        
        public DeliveryControllerTests(ServerFactory serverFactory)
        {
            _client = serverFactory.CreateClient();
        }
        
        #region GET

        [Fact]
        public async void Get_All_Should_Return_Success_If_Get_Successful()
        {
            var result = await _client.GetAsync(Endpoint);
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Get_Should_Return_Success_If_Get_Successful()
        {
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.SuccessValue}");
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Get_Should_Return_BadRequest_If_Get_Request_Is_Bad()
        {
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.BadRequestValue}");
            
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Get_Should_Return_Error_If_Get_Request_Is_Bad()
        {
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}");
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Get_Should_Return_NotFound_If_Resource_Not_Found()
        {
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.NotFound}");
            
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        #endregion

        #region POST
        
        [Fact]
        public async void POST_Should_Return_Success_If_POST_Successful()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.SuccessValue
            };
            
            var result = await _client.PostAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
        }
        
        [Fact]
        public async void POST_Should_Return_BadRequest_If_POST_Request_Is_Bad()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.BadRequestValue
            };
            
            var result = await _client.PostAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }
        
        [Fact]
        public async void POST_Should_Return_Error_If_POST_Request_Is_Bad()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.InternalErrorValue
            };
            
            var result = await _client.PostAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }

        #endregion
        
        #region PUT

        [Fact]
        public async void PUT_Should_Return_Success_If_PUT_Successful()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.SuccessValue
            };
            
            var result = await _client.PutAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
        }
        
        [Fact]
        public async void PUT_Should_Return_BadRequest_If_PUT_Request_Is_Bad()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.BadRequestValue
            };
            
            var result = await _client.PutAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }
        
        [Fact]
        public async void PUT_Should_Return_Error_If_PUT_Request_Is_Bad()
        {
            var requestObject = new DeliveryRecord
            {
                DeliveryId = TestConstants.InternalErrorValue
            };
            
            var result = await _client.PutAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.Default, "application/json"));
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }
        
        #endregion

        #region DELETE

        [Fact]
        public async void Delete_Should_Return_Success_If_Delete_Successful()
        {
            var result = await _client.DeleteAsync($"{Endpoint}/{TestConstants.SuccessValue}");
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Delete_Should_Return_BadRequest_If_Delete_Request_Is_Bad()
        {
            var result = await _client.DeleteAsync($"{Endpoint}/{TestConstants.BadRequestValue}");
            
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Delete_Should_Return_Error_If_Delete_Request_Is_Bad()
        {
            var result = await _client.DeleteAsync($"{Endpoint}/{TestConstants.InternalErrorValue}");
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Delete_Should_Return_NotFound_If_Resource_Not_Found()
        {
            var result = await _client.DeleteAsync($"{Endpoint}/{TestConstants.NotFound}");
            
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        
        #endregion
        
    }
}