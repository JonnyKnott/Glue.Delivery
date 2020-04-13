using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery.Enums;

namespace Glue.Delivery.Models.ServiceModels.Delivery
{
    [DynamoDBTable("Delivery")]
    public class DeliveryRecord
    {
        [DynamoDBHashKey]
        public string DeliveryId { get; set; }
        [DynamoDBProperty]
        public DeliveryState State { get; set; }
        
        [DynamoDBProperty]
        public DateTime AccessWindowStartTime { get; set; }
        
        [DynamoDBProperty]
        public DateTime AccessWindowEndTime { get; set; }

        [DynamoDBProperty]
        public Recipient Recipient { get; set; }
        [DynamoDBProperty]
        public Order Order { get; set; }

        public bool IsPending => State == DeliveryState.Created || State == DeliveryState.Approved;
    }
}