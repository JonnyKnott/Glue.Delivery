using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Castle.Core.Logging;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Glue.Delivery.Services.Test
{
    public class DeliveryStateServiceTests
    {
        private readonly DeliveryStateService _deliveryStateService;

        private readonly Mock<IDynamoDbRepository<DeliveryRecord>> _mockRepository;
        
        public DeliveryStateServiceTests()
        {
            _mockRepository = new Mock<IDynamoDbRepository<DeliveryRecord>>();
            var mockLogger = new Mock<ILogger<DeliveryStateService>>();
            _deliveryStateService = new DeliveryStateService(mockLogger.Object, _mockRepository.Object);
            
            _mockRepository.Setup(x => x.UpdateItem(It.IsAny<DeliveryRecord>()))
                .ReturnsAsync(ServiceResult.Succeeded);

            _mockRepository.Setup(x => x.GetItem(It.IsAny<string>()))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Failed(null, ErrorCodes.Status.NotFound));
        }

        [Fact]
        public async void ApproveDelivery_Should_Return_Success_If_Delivery_Is_Already_Approved()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Approved
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.ApproveDelivery(delivery.DeliveryId);

            Assert.True(result.Success);
        }
        
        [Fact]
        public async void ApproveDelivery_Should_Return_Success_And_Update_Delivery_If_Delivery_Is_Created()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Created
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.ApproveDelivery(delivery.DeliveryId);
            
            Assert.True(result.Success);
            
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.State == DeliveryState.Approved && record.DeliveryId == delivery.DeliveryId)), Times.Once);
        }
        
        [Fact]
        public async void ApproveDelivery_Should_Fail_Without_Updating_If_Delivery_In_Invalid_State()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.ApproveDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }

        [Fact]
        public async void ApproveDelivery_Should_Return_Failed_With_NotFound_If_Delivery_Does_Not_Exist()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };

            var result = await _deliveryStateService.ApproveDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            Assert.Contains(ErrorCodes.Status.NotFound, result.Errors);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }
        
        [Fact]
        public async void CancelDelivery_Should_Return_Success_If_Delivery_Is_Already_Cancelled()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Cancelled
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CancelDelivery(delivery.DeliveryId);

            Assert.True(result.Success);
        }
        
        [Fact]
        public async void CancelDelivery_Should_Return_Success_And_Update_Delivery_If_Delivery_Is_Pending()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Created
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CancelDelivery(delivery.DeliveryId);
            
            Assert.True(result.Success);
            
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.State == DeliveryState.Cancelled && record.DeliveryId == delivery.DeliveryId)), Times.Once);
        }
        
        [Fact]
        public async void CancelDelivery_Should_Fail_Without_Updating_If_Delivery_In_Invalid_State()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CancelDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }

        [Fact]
        public async void CancelDelivery_Should_Return_Failed_With_NotFound_If_Delivery_Does_Not_Exist()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };

            var result = await _deliveryStateService.CancelDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            Assert.Contains(ErrorCodes.Status.NotFound, result.Errors);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }
        
        [Fact]
        public async void CompleteDelivery_Should_Return_Success_If_Delivery_Is_Already_Completed()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CompleteDelivery(delivery.DeliveryId);

            Assert.True(result.Success);
        }
        
        [Fact]
        public async void CompleteDelivery_Should_Return_Success_And_Update_Delivery_If_Delivery_Is_Approved()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Approved
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CompleteDelivery(delivery.DeliveryId);
            
            Assert.True(result.Success);
            
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.State == DeliveryState.Completed && record.DeliveryId == delivery.DeliveryId)), Times.Once);
        }
        
        [Fact]
        public async void CompleteDelivery_Should_Fail_Without_Updating_If_Delivery_In_Invalid_State()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Expired
            };
            
            SetupSuccessfulGetForDelivery(delivery);
            
            var result = await _deliveryStateService.CompleteDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }

        [Fact]
        public async void CompleteDelivery_Should_Return_Failed_With_NotFound_If_Delivery_Does_Not_Exist()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString(),
                State = DeliveryState.Completed
            };

            var result = await _deliveryStateService.CompleteDelivery(delivery.DeliveryId);
            
            Assert.False(result.Success);
            Assert.Contains(ErrorCodes.Status.NotFound, result.Errors);
            _mockRepository.Verify(x => 
                x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)), Times.Never);
        }
        
        [Fact]
        public async void ExpireDeliveries_Should_Update_Returned_Items()
        {
            var expiredDeliveries = new List<DeliveryRecord>
            {
                new DeliveryRecord
                {
                    DeliveryId = Guid.NewGuid().ToString(),
                    AccessWindowEndTime = DateTime.Now,
                    State = DeliveryState.Created
                }
            };
            
            SetupSuccessfulGetForItems(expiredDeliveries);

            _mockRepository.Setup(x => x.UpdateItem(It.IsAny<DeliveryRecord>()))
                .ReturnsAsync(ServiceResult.Succeeded);

            var cutoff = DateTime.UtcNow;
            
            var result = await _deliveryStateService.ExpireDeliveries(cutoff);
            
            Assert.True(result.Success);
            
            _mockRepository.Verify(
                x => x.UpdateItem(It.Is<DeliveryRecord>(
                    record => record.State == DeliveryState.Expired && record.DeliveryId == expiredDeliveries.Single().DeliveryId)));
        }

        [Fact]
        public async void ExpireDeliveries_Should_Call_Repository_With_Query_Conditions_For_State_And_Expiry()
        {
            var expiredDeliveries = new List<DeliveryRecord>
            {
                new DeliveryRecord
                {
                    DeliveryId = Guid.NewGuid().ToString(),
                    AccessWindowEndTime = DateTime.Now,
                    State = DeliveryState.Created
                }
            };
            
            SetupSuccessfulGetForItems(expiredDeliveries);

            _mockRepository.Setup(x => x.UpdateItem(It.IsAny<DeliveryRecord>()))
                .ReturnsAsync(ServiceResult.Succeeded);

            var cutoff = DateTime.UtcNow;
            
            var result = await _deliveryStateService.ExpireDeliveries(cutoff);
            
            Assert.True(result.Success);
            
            _mockRepository.Verify(
                x => x
                    .GetItems(
                        It.Is<ICollection<QueryModel>>(
                            queries => 
                                queries.Count == 2 && 
                                queries.Any(
                                    query => 
                                        query.FieldName == nameof(DeliveryRecord.State) &&
                                        query.Values.Contains(DeliveryState.Created) &&
                                        query.Values.Contains(DeliveryState.Approved) &&
                                        query.Operator == ScanOperator.In
                                ) && 
                                queries.Any(
                                    query =>
                                        query.FieldName == nameof(DeliveryRecord.AccessWindowEndTime) &&
                                        query.Values.Contains(cutoff) &&
                                        query.Operator == ScanOperator.LessThan)
                                )), Times.Once);
        }

        private void SetupSuccessfulGetForItems(ICollection<DeliveryRecord> deliveryRecords)
        {
            _mockRepository.Setup(x => x.GetItems(It.IsAny<ICollection<QueryModel>>()))
                .ReturnsAsync(ServiceObjectResult<ICollection<DeliveryRecord>>.Succeeded(deliveryRecords));
        }

        private void SetupSuccessfulGetForDelivery(DeliveryRecord deliveryRecord)
        {
            _mockRepository.Setup(x => x.GetItem(deliveryRecord.DeliveryId))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Succeeded(deliveryRecord));
        }
    }
}