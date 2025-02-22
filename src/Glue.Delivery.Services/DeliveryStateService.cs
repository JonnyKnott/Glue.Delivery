﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.Services
{
    public class DeliveryStateService : IDeliveryStateService
    {
        private readonly ILogger<DeliveryStateService> _logger;
        private readonly IDynamoDbRepository<DeliveryRecord> _repository;

        public DeliveryStateService(ILogger<DeliveryStateService> logger, IDynamoDbRepository<DeliveryRecord> repository)
        {
            _logger = logger;
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
            
            if(deliveryRecordResult.Result.AccessWindowStartTime < DateTime.UtcNow)
                return ServiceResult.Failed(new List<string> {ErrorCodes.Status.BadRequest, ErrorCodes.Messages.InvalidStateProgression, "A delivery cannot be approved once the start time has passed"});

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

        public async Task<ServiceResult> ExpireDeliveries(DateTime cutoff)
        {
            var queryModels = new List<QueryModel>
            {
                new QueryModel(nameof(DeliveryRecord.State), ScanOperator.In,
                    DeliveryState.Approved, DeliveryState.Created),
                new QueryModel(nameof(DeliveryRecord.AccessWindowEndTime), ScanOperator.LessThan, cutoff)
            };

            var expiredDeliveries = await _repository.GetItems(queryModels);

            if(!expiredDeliveries.Success)
                return ServiceResult.Failed(expiredDeliveries.Errors);
            
            foreach (var delivery in expiredDeliveries.Result)
            {
                _logger.LogInformation($"Expiring delivery {delivery.DeliveryId} due to the access window passing at {delivery.AccessWindowEndTime}");
                
                delivery.State = DeliveryState.Expired;

                await _repository.UpdateItem(delivery);
            }
            
            return ServiceResult.Succeeded();
        }
    }
}