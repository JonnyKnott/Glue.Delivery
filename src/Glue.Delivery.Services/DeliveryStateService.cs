using System.Collections.Generic;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;

namespace Glue.Delivery.Services
{
    public class DeliveryStateService : IDeliveryStateService
    {
        private readonly IDynamoDbRepository<DeliveryRecord> _repository;

        public DeliveryStateService(IDynamoDbRepository<DeliveryRecord> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> ApproveDelivery(string deliveryId)
        {
            var deliveryRecordResult = await _repository.GetItem(deliveryId);
            
            if(!deliveryRecordResult.Success)
                return ServiceResult.Failed(deliveryRecordResult.Errors);
            
            if(deliveryRecordResult.Result == null)
                return ServiceResult.Failed(ErrorCodes.Status.NotFound);

            if(deliveryRecordResult.Result.State == DeliveryState.Approved)
                return ServiceResult.Succeeded();
            
            if(!deliveryRecordResult.Result.IsPending)
                return ServiceResult.Failed(new List<string> {ErrorCodes.Status.BadRequest, ErrorCodes.Messages.InvalidStateProgression});

            if (deliveryRecordResult.Result.State == DeliveryState.Created)
            {
                deliveryRecordResult.Result.State = DeliveryState.Approved;
                
                var updateResult = await _repository.UpdateItem(deliveryRecordResult.Result);
                
                if(!updateResult.Success)
                    return ServiceResult.Failed(updateResult.Errors);
            }

            return ServiceResult.Succeeded();
        }

        public async Task<ServiceResult> CompleteDelivery(string deliveryId)
        {
            var deliveryRecordResult = await _repository.GetItem(deliveryId);
            
            if(!deliveryRecordResult.Success)
                return ServiceResult.Failed(deliveryRecordResult.Errors);
            
            if(deliveryRecordResult.Result == null)
                return ServiceResult.Failed(ErrorCodes.Status.NotFound);
            
            if(deliveryRecordResult.Result.State == DeliveryState.Completed)
                return ServiceResult.Succeeded();

            if(deliveryRecordResult.Result.State != DeliveryState.Approved)
                return ServiceResult.Failed(new List<string> {ErrorCodes.Status.BadRequest, ErrorCodes.Messages.InvalidStateProgression});

            deliveryRecordResult.Result.State = DeliveryState.Completed;
                
            var updateResult = await _repository.UpdateItem(deliveryRecordResult.Result);
                
            return !updateResult.Success ? ServiceResult.Failed(updateResult.Errors) : ServiceResult.Succeeded();
        }

        public async Task<ServiceResult> CancelDelivery(string deliveryId)
        {
            var deliveryRecordResult = await _repository.GetItem(deliveryId);

            if (!deliveryRecordResult.Success)
                return ServiceResult.Failed(deliveryRecordResult.Errors);

            if (deliveryRecordResult.Result == null)
                return ServiceResult.Failed(ErrorCodes.Status.NotFound);

            if(deliveryRecordResult.Result.State == DeliveryState.Cancelled)
                return ServiceResult.Succeeded();
            
            if (!deliveryRecordResult.Result.IsPending)
                return ServiceResult.Failed(new List<string>
                    {ErrorCodes.Status.BadRequest, ErrorCodes.Messages.InvalidStateProgression});

            deliveryRecordResult.Result.State = DeliveryState.Cancelled;

            var updateResult = await _repository.UpdateItem(deliveryRecordResult.Result);

            return !updateResult.Success ? ServiceResult.Failed(updateResult.Errors) : ServiceResult.Succeeded();
        }
    }
}