using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using AutoMapper;
using Glue.Delivery.Core.Configuration;
using Glue.Delivery.Data;
using Glue.Delivery.Models.ServiceModels.Delivery;
using Glue.Delivery.Services;
using Glue.Delivery.WebApi.Mapping;
using Glue.Delivery.WebApi.Middleware;
using Glue.Delivery.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionConfiguration = _configuration.GetSection("Connections").Get<ConnectionConfiguration>();
            
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddLogging(builder => { builder.AddConsole(); });

            services.AddHealthChecks();
            
            services.AddAutoMapper(
                x => x.AddProfile<Mappings>(), 
                typeof(Startup));

            services
                .AddScoped<IDeliveryService, DeliveryService>()
                .AddScoped<IDeliveryStateService, DeliveryStateService>()
                .AddSingleton<IDynamoDbRepository<DeliveryRecord>, DynamoDbRepository<DeliveryRecord>>();

            if (_webHostEnvironment.IsDevelopment())
            {
                services.AddDefaultAWSOptions(new AWSOptions());

                var dynamoDbClient = new AmazonDynamoDBClient(
                    new BasicAWSCredentials(
                        _configuration["AWS_ACCESS_KEY_ID"], 
                        _configuration["AWS_SECRET_ACCESS_KEY"])
                    , new AmazonDynamoDBConfig
                    {
                        UseHttp = true,
                        ServiceURL = connectionConfiguration.DynamoDb
                    });
                
                services
                    .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
                    .AddHostedService<DevelopmentTableSetup>()
                    .AddTransient<IDynamoDBContext, DynamoDBContext>();
            }
            else
            {
                services
                    .AddAWSService<IAmazonDynamoDB>()
                    .AddTransient<IDynamoDBContext, DynamoDBContext>();

            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes = new Dictionary<HealthStatus, int>
                {
                    { HealthStatus.Healthy, StatusCodes.Status200OK },
                    { HealthStatus.Degraded, StatusCodes.Status200OK },
                    { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }

                }
            });
            
            app.UseRouting();

            app.UseMiddleware<MappingExceptionMiddleware>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}