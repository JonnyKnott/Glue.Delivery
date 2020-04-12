using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Glue.Delivery.WebApi.Mapping.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Glue.Delivery.WebApi.Middleware
{
    public class MappingExceptionMiddleware
    {
        private readonly ILogger<MappingExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public MappingExceptionMiddleware(ILogger<MappingExceptionMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (AutoMapperMappingException autoMapperMappingException)
            {
                _logger.LogError(autoMapperMappingException, "Error occurred performing mapping between Api and Service models");

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(CreateMappingErrorResponse(autoMapperMappingException)));
            }
        }

        private object CreateMappingErrorResponse(AutoMapperMappingException mappingException)
        {
            return new {Error = "InvalidRequest", ModelErrors = new {Fields = mappingException.FailedFields()}};
        }
    }
}