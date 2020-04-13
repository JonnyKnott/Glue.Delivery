using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;

namespace Glue.Delivery.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDynamoDbRepository<DeliveryRecord> _repository;

        public DeliveryService(IDynamoDbRepository<DeliveryRecord> repository)
        {
            _repository = repository;
        }
        
        public async Task<ServiceObjectResult<DeliveryRecord>> Create(DeliveryRecord delivery)
        {
            delivery.DeliveryId = Guid.NewGuid().ToString();

            var saveResult = await _repository.PutItem(delivery);
            
            return saveResult.Success ? 
                ServiceObjectResult<DeliveryRecord>.Succeeded(delivery)
                : ServiceObjectResult<DeliveryRecord>.Failed(null, saveResult.Errors);
        }

        public async Task<ServiceObjectResult<DeliveryRecord>> Update(DeliveryRecord delivery)
        {
            var result = await _repository.UpdateItem(delivery);
            
            return !result.Success ? 
                ServiceObjectResult<DeliveryRecord>.Failed(null, result.Errors) 
                : ServiceObjectResult<DeliveryRecord>.Succeeded(delivery);
        }

        public async Task<ServiceObjectResult<ICollection<DeliveryRecord>>> Select(DeliveryState? state = null)
        {
            var result = state.HasValue ? 
                await _repository.GetItems(new List<QueryModel>
                    {
                        new QueryModel(nameof(DeliveryRecord.State), ScanOperator.Equal, state)
                    })
                : await _repository.GetItems();

            return result.Success ? 
                ServiceObjectResult<ICollection<DeliveryRecord>>.Succeeded(result.Result)
                : ServiceObjectResult<ICollection<DeliveryRecord>>.Failed(null, result.Errors);
        }

        public async Task<ServiceObjectResult<DeliveryRecord>> Select(string deliveryId)
        {
            var result = await _repository.GetItem(deliveryId);
            
            return !result.Success ? 
                ServiceObjectResult<DeliveryRecord>.Failed(null, result.Errors)
                : ServiceObjectResult<DeliveryRecord>.Succeeded(result.Result);
        }

        public async Task<ServiceResult> Delete(string deliveryId)
        {
            var result = await _repository.DeleteItem(deliveryId);
            
            return result.Success ? ServiceResult.Succeeded() : result;
        }
    }
}