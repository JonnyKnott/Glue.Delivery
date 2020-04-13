using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Glue.Delivery.Core.Configuration;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Glue.Delivery.DeliveryState.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await BuildHost(args)
                    .RunConsoleAsync();
            }
            catch (Exception applicationException)
            {
                Log.Fatal(applicationException, "An unexpected error occurred");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder BuildHost(string[] args)
        {
            Log.Information("Building service dependencies");
            
            return new HostBuilder()
                .ConfigureHostConfiguration(configHost => { configHost.AddEnvironmentVariables(); })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", true);
                    builder.AddJsonFile("appsettings.Development.json", true);
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionConfiguration =
                        context.Configuration.GetSection("Connections").Get<ConnectionConfiguration>();

                    var scheduledServiceConfiguration =
                        context.Configuration.GetSection("ScheduledService").Get<ScheduledServiceConfiguration>();

                    services
                        .AddSingleton(scheduledServiceConfiguration)
                        .AddScoped<IDeliveryService, DeliveryService>()
                        .AddScoped<IDeliveryStateService, DeliveryStateService>()
                        .AddSingleton<IDynamoDbRepository<DeliveryRecord>, DynamoDbRepository<DeliveryRecord>>()
                        .AddHostedService<StateUpdateHostedService>();
                    
                    services.AddLogging(builder => { builder.AddConsole(); });
                    
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        services.AddDefaultAWSOptions(new AWSOptions());

                        var dynamoDbClient = new AmazonDynamoDBClient(
                            new BasicAWSCredentials(
                                context.Configuration["AWS_ACCESS_KEY_ID"], 
                                context.Configuration["AWS_SECRET_ACCESS_KEY"])
                            , new AmazonDynamoDBConfig
                            {
                                UseHttp = true,
                                ServiceURL = connectionConfiguration.DynamoDb
                            });

                        services
                            .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
                            .AddTransient<IDynamoDBContext, DynamoDBContext>();
                    }
                    else
                    {
                        services
                            .AddAWSService<IAmazonDynamoDB>()
                            .AddTransient<IDynamoDBContext, DynamoDBContext>();
                    }
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration.WriteTo.Console();
                });
        } 
    }
}