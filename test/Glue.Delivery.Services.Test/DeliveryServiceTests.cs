using System;
using System.Collections.Generic;
using System.Linq;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;
using Moq;
using Xunit;

namespace Glue.Delivery.Services.Test
{
    
    
    public class DeliveryServiceTests
    {
        private readonly DeliveryService _deliveryService;
        private readonly Mock<IDynamoDbRepository<DeliveryRecord>> _mockRepository;

        private const string DefaultDeliveryId = "TestValue";

        public DeliveryServiceTests()
        {
            _mockRepository = new Mock<IDynamoDbRepository<DeliveryRecord>>();
            
            _deliveryService = new DeliveryService(_mockRepository.Object);
            
            SetupRepositorySuccess(new DeliveryRecord
            {
                DeliveryId = DefaultDeliveryId
            });
        }
        
        [Fact]
        public async void Create_Should_Call_PutItem_With_New_DeliveryId()
        {
            var delivery = new DeliveryRecord();
            
            var result = await _deliveryService.Create(delivery);
            
            Assert.True(result.Success);
            Assert.NotNull(result.Result.DeliveryId);
            _mockRepository
                .Verify(
                    x => x.PutItem(
                        It.Is<DeliveryRecord>(record => record.DeliveryId == result.Result.DeliveryId)
                        )
                    );
        }
        
        [Fact]
        public async void Create_Should_Return_Failure_If_Put_Fails()
        {
            var error = "Database Error";
            
            var delivery = new DeliveryRecord
            {
                Order = new Order{ OrderNumber = Guid.NewGuid().ToString()}
            };

            _mockRepository.Setup(x =>
                    x.PutItem(It.Is<DeliveryRecord>(record => record.Order.OrderNumber == delivery.Order.OrderNumber)))
                .ReturnsAsync(ServiceResult.Failed(error));
            
            var result = await _deliveryService.Create(delivery);
            
            Assert.False(result.Success);
            Assert.Null(result.Result);
            _mockRepository
                .Verify(
                    x => x.PutItem(
                        It.Is<DeliveryRecord>(record => record.Order.OrderNumber == delivery.Order.OrderNumber)
                    )
                );
        }

        [Fact]
        public async void Update_Should_Use_Existing_DeliveryId()
        {
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString()
            };

            var result = await _deliveryService.Update(delivery);
            
            Assert.True(result.Success);
            _mockRepository
                .Verify(
                    x => x.UpdateItem(
                        It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)
                    )
                );
        }

        [Fact]
        public async void Update_Should_Return_Errors_When_Update_Fails()
        {
            var error = "Error";
            
            var delivery = new DeliveryRecord
            {
                DeliveryId = Guid.NewGuid().ToString()
            };
            
            _mockRepository.Setup(x =>
                    x.UpdateItem(It.Is<DeliveryRecord>(record => record.DeliveryId == delivery.DeliveryId)))
                .ReturnsAsync(ServiceResult.Failed(error));

            
            var result = await _deliveryService.Update(delivery);
            
            Assert.False(result.Success);
            Assert.Contains(error, result.Errors);
        }

        [Fact]
        public async void SelectById_Should_Return_Result_If_Found()
        {
            var result = await _deliveryService.Select(DefaultDeliveryId);
            
            Assert.True(result.Success);
            Assert.Equal(DefaultDeliveryId, result.Result.DeliveryId);
        }

        [Fact]
        public async void SelectById_Should_Return_Errors_If_Select_Fails()
        {
            var error = "Error";
            
            var deliveryId = Guid.NewGuid().ToString();
            
            _mockRepository.Setup(x =>
                    x.GetItem(deliveryId))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Failed(null, error));

            
            var result = await _deliveryService.Select(deliveryId);
            
            Assert.False(result.Success);
            Assert.Contains(error, result.Errors);
        }

        [Fact]
        public async void Select_Should_Query_By_State()
        {
            var result = await _deliveryService.Select(DeliveryState.Approved);
            
            _mockRepository.Verify(x => 
                x.GetItems(
                    It.Is<ICollection<QueryModel>>(
                        queryModels => queryModels.Count == 1)
                    ), 
                Times.Once);
        }
        
        [Fact]
        public async void SelectAll_Should_Return_Result_If_Found()
        {
            var result = await _deliveryService.Select();
            
            Assert.True(result.Success);
            Assert.Single(result.Result);
            Assert.Equal(DefaultDeliveryId, result.Result.Single().DeliveryId);
        }

        [Fact]
        public async void Delete_Should_Return_Result_If_Found()
        {
            var result = await _deliveryService.Delete(DefaultDeliveryId);
            
            Assert.True(result.Success);
        }

        [Fact]
        public async void Delete_Should_Return_Errors_If_Select_Fails()
        {
            var error = "Error";
            
            var deliveryId = Guid.NewGuid().ToString();
            
            _mockRepository.Setup(x =>
                    x.DeleteItem(deliveryId))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Failed(null, error));

            
            var result = await _deliveryService.Delete(deliveryId);
            
            Assert.False(result.Success);
            Assert.Contains(error, result.Errors);
        }

        private void SetupRepositorySuccess(DeliveryRecord record)
        {
            _mockRepository.Setup(x => x.PutItem(It.IsAny<DeliveryRecord>()))
                .ReturnsAsync(ServiceResult.Succeeded);

            _mockRepository.Setup(x => x.GetItems(It.IsAny<ICollection<QueryModel>>()))
                .ReturnsAsync(ServiceObjectResult<ICollection<DeliveryRecord>>.Succeeded(new List<DeliveryRecord>{ record }));
            
            _mockRepository.Setup(x => x.GetItems(default))
                .ReturnsAsync(ServiceObjectResult<ICollection<DeliveryRecord>>.Succeeded(new List<DeliveryRecord>{ record }));
            
            _mockRepository.Setup(x => x.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(ServiceResult.Succeeded);

            _mockRepository.Setup(x => x.GetItem(It.IsAny<string>()))
                .ReturnsAsync(ServiceObjectResult<DeliveryRecord>.Succeeded(record));
            
            _mockRepository.Setup(x => x.UpdateItem(It.IsAny<DeliveryRecord>()))
                .ReturnsAsync(ServiceResult.Succeeded);
        }
    }
}