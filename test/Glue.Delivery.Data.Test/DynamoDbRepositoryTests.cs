using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Moq;
using Xunit;

namespace Glue.Delivery.Data.Test
{
    public class TestEntity
    {
        [DynamoDBHashKey]
        public string PartitionKey { get; set; }
    }
    
    public class DynamoDbRepositoryTests
    {
        private readonly DynamoDbRepository<TestEntity> _repository;
        private Mock<IDynamoDBContext> _mockContext;
        
        public DynamoDbRepositoryTests()
        {
            _mockContext = new Mock<IDynamoDBContext>();
            
            _repository = new DynamoDbRepository<TestEntity>(_mockContext.Object);

            SetupSuccessfulContextCalls();
        }

        [Fact]
        public async void GetItem_Should_Call_Context_With_Provided_Key()
        {
            var partitionKey = Guid.NewGuid().ToString();

            var result = await _repository.GetItem(partitionKey);
            
            Assert.True(result.Success);
            _mockContext.Verify(x => x.LoadAsync<TestEntity>(partitionKey, default), Times.Once);
        }
        
        [Fact]
        public async void UpdateItem_Should_Infer_HashKey_And_Check_Item_Exists()
        {
            var partitionKey = Guid.NewGuid().ToString();
            
            var entity = new TestEntity{ PartitionKey = partitionKey };

            var result = await _repository.UpdateItem(entity);
            
            Assert.True(result.Success);
            _mockContext.Verify(x => x.LoadAsync<TestEntity>(partitionKey, default), Times.Once);
        }
        
        [Fact]
        public async void UpdateItem_Should_Perform_Save_If_Item_Exists()
        {
            var partitionKey = Guid.NewGuid().ToString();
            
            var entity = new TestEntity{ PartitionKey = partitionKey };

            var result = await _repository.UpdateItem(entity);
            
            Assert.True(result.Success);
            _mockContext.Verify(x => x.SaveAsync(It.Is<TestEntity>(ent => ent.PartitionKey == partitionKey), default), Times.Once);
        }
        
        [Fact]
        public async void DeleteItem_Should_Perform_Save_If_Item_Exists()
        {
            var partitionKey = Guid.NewGuid().ToString();

            var result = await _repository.DeleteItem(partitionKey);
            
            Assert.True(result.Success);
            _mockContext.Verify(x => x.DeleteAsync<TestEntity>(partitionKey, default), Times.Once);
        }

        [Fact]
        public async void CreateItem_Should_Perform_Save_Only()
        {
            var partitionKey = Guid.NewGuid().ToString();
            
            var entity = new TestEntity{ PartitionKey = partitionKey };

            var result = await _repository.PutItem(entity);
            
            Assert.True(result.Success);
            _mockContext.Verify(x => x.LoadAsync<TestEntity>(It.Is<TestEntity>(ent => ent.PartitionKey == partitionKey), default), Times.Never);
            _mockContext.Verify(x => x.SaveAsync(It.Is<TestEntity>(ent => ent.PartitionKey == partitionKey), default), Times.Once);

        }

        private void SetupSuccessfulContextCalls()
        {
            
            //TODO ScanAsync cannot be properly mocked right now due to return AsyncSearch object with internal only constructor.
            //Issue ongoing with possible workaround https://github.com/aws/aws-sdk-net/issues/772

            _mockContext.Setup(x => x.DeleteAsync<TestEntity>(It.IsAny<string>(), default));
            _mockContext.Setup(x => x.LoadAsync<TestEntity>(It.IsAny<string>(), default))
                .ReturnsAsync(new TestEntity());
            _mockContext.Setup(x => x.SaveAsync(It.IsAny<TestEntity>(), default));
        }
    }
}