using System.Linq;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Models.ApiModels.Auth;
using Glue.Delivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route( "api/v{version:apiVersion}/{controller}" )]
    public class DeliveryStateController : ControllerBase
    {
        private readonly ILogger<DeliveryStateController> _logger;
        private readonly IDeliveryStateService _deliveryStateService;

        public DeliveryStateController(ILogger<DeliveryStateController> logger, IDeliveryStateService deliveryStateService)
        {
            _logger = logger;
            _deliveryStateService = deliveryStateService;
        }
        
        [Authorize(Roles = 
            AuthorizationConstants.Claims.PartnerClaim + "," + 
            AuthorizationConstants.Claims.SystemClaim + "," + 
            AuthorizationConstants.Claims.UserClaim)]
        [HttpGet("{deliveryId}/Cancel")]
        public async Task<IActionResult> Cancel([FromRoute]string deliveryId)
        {
            var result = await _deliveryStateService.CancelDelivery(deliveryId); 

            return result.Success ?
                Ok()
                : GenerateResultFromServiceResult(result);
        }
        
        [Authorize(Roles = 
            AuthorizationConstants.Claims.PartnerClaim + "," + 
            AuthorizationConstants.Claims.SystemClaim)]
        [HttpGet("{deliveryId}/Complete")]
        public async Task<IActionResult> Complete([FromRoute]string deliveryId)
        {
            var result = await _deliveryStateService.CompleteDelivery(deliveryId);

            return result.Success ?
                Ok()
                : GenerateResultFromServiceResult(result);
        }
        
        [Authorize(Roles =
            AuthorizationConstants.Claims.SystemClaim + "," + 
            AuthorizationConstants.Claims.UserClaim)]
        [HttpGet("{deliveryId}/Approve")]
        public async Task<IActionResult> Approve([FromRoute]string deliveryId)
        {
            var result = await _deliveryStateService.ApproveDelivery(deliveryId);

            return result.Success ?
                Ok()
                : GenerateResultFromServiceResult(result);
        }
        
        private IActionResult GenerateResultFromServiceResult(
            ServiceResult serviceResult)
        {
            if (serviceResult.Errors.Contains(ErrorCodes.Status.UnexpectedError))
                return StatusCode(500, serviceResult.Errors);
            
            if (serviceResult.Errors.Contains(ErrorCodes.Status.BadRequest))
                return BadRequest(serviceResult.Errors);
            
            if (serviceResult.Errors.Contains(ErrorCodes.Status.NotFound))
                return NotFound(serviceResult.Errors);

            return serviceResult.Errors.Any() ? 
                StatusCode(500, serviceResult.Errors)
                : StatusCode(500, "The request could not be processed due to an unknown reason.");
        }
        
    }
}