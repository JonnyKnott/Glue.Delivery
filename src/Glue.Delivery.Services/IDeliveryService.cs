using System.Collections.Generic;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;

namespace Glue.Delivery.Services
{
    public interface IDeliveryService
    {
        Task<ServiceObjectResult<DeliveryRecord>> Create(DeliveryRecord delivery);
        Task<ServiceObjectResult<ICollection<DeliveryRecord>>> Select(DeliveryState? state = null);
        Task<ServiceObjectResult<DeliveryRecord>> Select(string deliveryId);
        Task<ServiceObjectResult<DeliveryRecord>> Update(DeliveryRecord delivery);
        Task<ServiceResult> Delete(string deliveryId);
    }
}