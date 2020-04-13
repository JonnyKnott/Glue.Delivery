using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Glue.Delivery.Core.Helpers;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Models.ApiModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;
using Glue.Delivery.Services;
using Glue.Delivery.WebApi.Mapping.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceModels = Glue.Delivery.Models.ServiceModels;
using ApiModels = Glue.Delivery.Models.ApiModels;

namespace Glue.Delivery.WebApi.Controllers
{
    [ApiController]
    [Route( "api/v{version:apiVersion}/{controller}" )]
    public class DeliveryController : ControllerBase
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly IMapper _mapper;
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(ILogger<DeliveryController> logger, IMapper mapper, IDeliveryService deliveryService)
        {
            _logger = logger;
            _mapper = mapper;
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]DeliveryState? state = null)
        {
            var result = await _deliveryService.Select(state);

            return result.Success ? 
                Ok(_mapper.Map<List<DeliveryResponse>>(result.Result)) 
                : GenerateResultFromServiceResult(result);
        }

        [HttpGet("{deliveryId}")]
        public async Task<IActionResult> GetById(string deliveryId)
        {
            var result = await _deliveryService.Select(deliveryId);
            
            return result.Success ? 
                Ok(_mapper.Map<DeliveryResponse>(result.Result)) 
                : GenerateResultFromServiceResult(result);
        }
        
        [HttpPut]
        public async Task<IActionResult> Put(DeliveryRequest request)
        {
            if (request.DeliveryId == null)
                return BadRequest(new ApiModels.ErrorResponse( new List<string>{ "A DeliveryId is required to update a delivery" }));
            
            var serviceModel = _mapper.Map<DeliveryRecord>(request);

            var result = await _deliveryService.Update(serviceModel);

            return result.Success ? 
                Ok(_mapper.Map<DeliveryResponse>(result.Result)) 
                : GenerateResultFromServiceResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(DeliveryRequest request)
        {
            var serviceModel = _mapper.Map<DeliveryRecord>(request);

            serviceModel.State = DeliveryState.Created;
            
            var result = await _deliveryService.Create(serviceModel);
            
            return result.Success ?
                Ok(_mapper.Map<DeliveryResponse>(result.Result))
                : GenerateResultFromServiceResult(result);
        }

        [HttpDelete("{deliveryId}")]
        public async Task<IActionResult> Delete(string deliveryId)
        {
            var result = await _deliveryService.Delete(deliveryId);
            
            
            
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
        
        private IActionResult GenerateResultFromServiceResult<TResultType>(
            ServiceObjectResult<TResultType> serviceObjectResult)
        {

            if (serviceObjectResult.Errors.Contains(ErrorCodes.Status.UnexpectedError))
                return StatusCode(500, serviceObjectResult.Errors);
            
            if (serviceObjectResult.Errors.Contains(ErrorCodes.Status.BadRequest))
                return BadRequest(new ApiModels.ErrorResponse(serviceObjectResult.Errors));
            
            if (serviceObjectResult.Errors.Contains(ErrorCodes.Status.NotFound))
                return NotFound();

            return serviceObjectResult.Errors.Any() ? 
                StatusCode(500, serviceObjectResult.Errors) 
                : StatusCode(500, "The request could not be processed due to an unknown reason.");
        }
    }
}