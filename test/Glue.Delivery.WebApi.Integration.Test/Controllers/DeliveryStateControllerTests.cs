using System.Net.Http;
using System.Net.Http.Headers;
using Glue.Delivery.Models.ApiModels.Auth;
using Glue.Delivery.WebApi.Integration.Test.Helpers;
using Glue.Delivery.WebApi.Integration.Test.Infrastructure;
using Glue.Delivery.WebApi.Integration.Test.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Glue.Delivery.WebApi.Integration.Test.Controllers
{
    public class DeliveryStateControllerTests : IClassFixture<ServerFactory>
    {
        private readonly HttpClient _client;

        private const string Endpoint = "api/v1/deliverystate";

        private string _userToken;
        private string _partnerToken;
        
        public DeliveryStateControllerTests(ServerFactory serverFactory)
        {
            _client = serverFactory.CreateClient();
            
            _userToken = AuthorizationHelpers.GenerateTokenForRole(AuthorizationConstants.Claims.UserClaim);
            _partnerToken = AuthorizationHelpers.GenerateTokenForRole(AuthorizationConstants.Claims.PartnerClaim);
        }

        #region Approve
        
        [Fact]
        public async void Approve_Should_Return_Success_If_Service_Succeeds()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _userToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.SuccessValue}/Approve");
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Approve_Should_Return_NotFound_If_Delivery_Does_Not_Exist()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _userToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.NotFound}/Approve");
            
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Approve_Should_Return_Error_If_ServiceFails()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _userToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}/Approve");
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }

        [Fact]
        public async void Approve_Should_Return_Forbidden_For_Partner()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}/Approve");
            
            Assert.Equal(StatusCodes.Status403Forbidden, (int)result.StatusCode);
        }
        
        #endregion
        
        #region Cancel
        
        [Fact]
        public async void Cancel_Should_Return_Success_If_Service_Succeeds()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.SuccessValue}/Cancel");
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Cancel_Should_Return_NotFound_If_Delivery_Does_Not_Exist()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.NotFound}/Cancel");
            
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Cancel_Should_Return_Error_If_ServiceFails()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}/Cancel");
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }
        
        #endregion
        
        #region Complete
        
        [Fact]
        public async void Complete_Should_Return_Success_If_Service_Succeeds()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.SuccessValue}/Complete");
            
            Assert.True(result.IsSuccessStatusCode);
        }
        
        [Fact]
        public async void Complete_Should_Return_NotFound_If_Delivery_Does_Not_Exist()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.NotFound}/Complete");
            
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Complete_Should_Return_Error_If_ServiceFails()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _partnerToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}/Complete");
            
            Assert.Equal(StatusCodes.Status500InternalServerError, (int)result.StatusCode);
        }
        
        [Fact]
        public async void Complete_Should_Return_Forbidden_For_User()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _userToken);
            
            var result = await _client.GetAsync($"{Endpoint}/{TestConstants.InternalErrorValue}/Complete");
            
            Assert.Equal(StatusCodes.Status403Forbidden, (int)result.StatusCode);
        }
        
        #endregion
    }
}