using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Glue.Delivery.Core;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.WebApi.Services
{
    public class DevelopmentTableSetup : IHostedService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DevelopmentTableSetup> _logger;

        public DevelopmentTableSetup(IAmazonDynamoDB dynamoDbClient, ILogger<DevelopmentTableSetup> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tables = await _dynamoDbClient.ListTablesAsync(CancellationToken.None);

            if (!tables.TableNames.Contains(Constants.DynamoDb.DeliveryTableName))
                await CreateDeliveryTable();
        }

        private async Task CreateDeliveryTable()
        {
            _logger.LogInformation($"Creating new DynamoDb table {Constants.DynamoDb.DeliveryTableName}");

            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = nameof(DeliveryRecord.DeliveryId),
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = Constants.DynamoDb.DeliveryTablePartitionKey,
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput()
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 10
                },
                TableName = Constants.DynamoDb.DeliveryTableName
            };
            
            await _dynamoDbClient.CreateTableAsync(request);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
