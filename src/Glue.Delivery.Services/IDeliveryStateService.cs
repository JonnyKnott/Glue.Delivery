using System;
using System.Threading.Tasks;
using Glue.Delivery.Core.Models;

namespace Glue.Delivery.Services
{
    public interface IDeliveryStateService
    {
        Task<ServiceResult> ApproveDelivery(string deliveryId);
        Task<ServiceResult> CompleteDelivery(string deliveryId);
        Task<ServiceResult> CancelDelivery(string deliveryId);
        Task<ServiceResult> ExpireDeliveries(DateTime cutoff);
    }
}