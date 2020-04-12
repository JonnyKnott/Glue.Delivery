using System;

namespace Glue.Delivery.Models.ApiModels.Delivery
{
    public class DeliveryRecord
    {
        public string DeliveryId { get; set; }
        public string State { get; set; }
        public AccessWindow AccessWindow { get; set; }
        public Recipient Recipient { get; set; }
        public Order Order { get; set; }
    }
}