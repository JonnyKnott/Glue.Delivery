using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Services;

namespace Glue.Delivery.WebApi.Integration.Test.Services
{
    public class TestDeliveryService : IDeliveryService
    {
        public Task<ServiceObjectResult<DeliveryRecord>> Create(DeliveryRecord delivery)
        {
            return Task.FromResult(CreateObjectResponseForRequest<DeliveryRecord>(delivery.DeliveryId));
        }

        public Task<ServiceObjectResult<ICollection<DeliveryRecord>>> Select()
        {
            return Task.FromResult(
                ServiceObjectResult<ICollection<DeliveryRecord>>.Succeeded(new List<DeliveryRecord>()));
        }

        public Task<ServiceObjectResult<DeliveryRecord>> Select(string deliveryId)
        {
            return Task.FromResult(CreateObjectResponseForRequest<DeliveryRecord>(deliveryId));
        }

        public Task<ServiceObjectResult<DeliveryRecord>> Update(DeliveryRecord delivery)
        {
            return Task.FromResult(CreateObjectResponseForRequest<DeliveryRecord>(delivery.DeliveryId));
        }

        public Task<ServiceResult> Delete(string deliveryId)
        {
            return Task.FromResult(CreateResponseForRequest(deliveryId));
        }

        private ServiceObjectResult<TResponseType> CreateObjectResponseForRequest<TResponseType>(string deliveryId)
        where TResponseType : class, new()
        {
            switch (deliveryId)
            {
                case TestConstants.BadRequestValue:
                    return ServiceObjectResult<TResponseType>.Failed(null, ErrorCodes.Status.BadRequest);
                case TestConstants.InternalErrorValue:
                    return ServiceObjectResult<TResponseType>.Failed(null, ErrorCodes.Status.UnexpectedError);
                case TestConstants.NotFound:
                    return ServiceObjectResult<TResponseType>.Failed(null, ErrorCodes.Status.NotFound);
                case TestConstants.SuccessValue:
                    return ServiceObjectResult<TResponseType>.Succeeded(new TResponseType());
                default:
                    throw new NotImplementedException();
            }
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