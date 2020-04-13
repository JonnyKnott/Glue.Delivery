namespace Glue.Delivery.Core
{
    public static class Constants
    {
        public static class DynamoDb
        {
            public const string DeliveryTableName = "Delivery";
            public const string DeliveryTablePartitionKey = "DeliveryId";
        }
    }
}