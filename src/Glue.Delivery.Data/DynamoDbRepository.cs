using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Glue.Delivery.Core.Models;

namespace Glue.Delivery.Data
{
    public class DynamoDbRepository<TDataType> : IDynamoDbRepository<TDataType>
        where TDataType : class
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbRepository(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }
        
        public async Task<ServiceResult> PutItem(TDataType dataEntity)
        {
            await _dynamoDbContext.SaveAsync(dataEntity);
            
            return ServiceResult.Succeeded();
        }

        public async Task<ServiceResult> UpdateItem(TDataType dataEntity)
        {
            var hashKeyProperty = typeof(TDataType).GetProperties()
                .Single(prop => prop.IsDefined(typeof(DynamoDBHashKeyAttribute), false));

            var partitionKey = hashKeyProperty.GetValue(dataEntity) as string;

            var record = await _dynamoDbContext.LoadAsync<TDataType>(partitionKey);
            
            if(record == null)
                return ServiceResult.Failed(ErrorCodes.Status.NotFound);

            await _dynamoDbContext.SaveAsync<TDataType>(dataEntity);
            
            return ServiceResult.Succeeded();
        }

        public async Task<ServiceObjectResult<ICollection<TDataType>>> GetItems(ICollection<QueryModel> queryModels = null)
        {
            var scanConditions = queryModels == null ? 
                new List<ScanCondition>() 
                : queryModels.Select(x => new ScanCondition(
                x.FieldName, x.Operator, x.Value)).ToList();
            
            var results = await _dynamoDbContext.ScanAsync<TDataType>(scanConditions).GetRemainingAsync();

            return ServiceObjectResult<ICollection<TDataType>>.Succeeded(results);
        }

        public async Task<ServiceObjectResult<TDataType>> GetItem(string partitionKey)
        {
            var result = await _dynamoDbContext.LoadAsync<TDataType>(partitionKey);
            
            return result != null ? 
                ServiceObjectResult<TDataType>.Succeeded(result)
                : ServiceObjectResult<TDataType>.Failed(null, ErrorCodes.Status.NotFound);
        }

        public async Task<ServiceResult> DeleteItem(string partitionKey)
        {
            await _dynamoDbContext.DeleteAsync<TDataType>(partitionKey);
            
            return ServiceResult.Succeeded();
        }
    }
}