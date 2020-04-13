using System;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;
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
            
            _deliveryStateService = new DeliveryStateService(_mockRepository.Object);
            
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

        private void SetupSuccessfulGetForDelivery(DeliveryRecord deliveryRecord)
        {
            _mockRepository.Setup(x => x.GetItem(deliveryRecord.DeliveryId))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Succeeded(deliveryRecord));
        }
    }
}