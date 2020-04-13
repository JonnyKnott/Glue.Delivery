using System;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Services;

namespace Glue.Delivery.WebApi.Integration.Test.Services
{
    public class TestDeliveryStateService : IDeliveryStateService
    {
        public Task<ServiceResult> ApproveDelivery(string deliveryId)
        {
            return Task.FromResult(CreateResponseForRequest(deliveryId));
        }

        public Task<ServiceResult> CompleteDelivery(string deliveryId)
        {
            return Task.FromResult(CreateResponseForRequest(deliveryId));
        }

        public Task<ServiceResult> CancelDelivery(string deliveryId)
        {
            return Task.FromResult(CreateResponseForRequest(deliveryId));
        }
        
        private ServiceResult CreateResponseForRequest(string deliveryId)
        {
            switch (deliveryId)
            {
                case TestConstants.BadRequestValue:
                    return ServiceResult.Failed(ErrorCodes.Status.BadRequest);
                case TestConstants.InternalErrorValue:
                    return ServiceResult.Failed(ErrorCodes.Status.UnexpectedError);
                case TestConstants.NotFound:
                    return ServiceResult.Failed(ErrorCodes.Status.NotFound);
                case TestConstants.SuccessValue:
                    return ServiceResult.Succeeded();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}