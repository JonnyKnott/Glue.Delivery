using System.Collections.Generic;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;

namespace Glue.Delivery.Data
{
    public interface IDynamoDbRepository<TDataType>
    where TDataType : class
    {
        Task<ServiceResult> PutItem(TDataType dataEntity);
        Task<ServiceResult> UpdateItem(TDataType dataEntity);
        Task<ServiceObjectResult<ICollection<TDataType>>> GetItems(ICollection<QueryModel> queryModels = null);
        Task<ServiceObjectResult<TDataType>> GetItem(string partitionKey);
        Task<ServiceResult> DeleteItem(string partitionKey);
    }
}