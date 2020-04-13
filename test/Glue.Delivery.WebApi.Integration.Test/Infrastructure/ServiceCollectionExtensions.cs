using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Glue.Delivery.WebApi.Integration.Test.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceServiceType<TInterface, TNewImplementation>(this IServiceCollection services)
            where TInterface : class
            where TNewImplementation : class, TInterface
        {
            var registeredService = services.SingleOrDefault(
                service => service.ServiceType == typeof(TInterface));

            services.Remove(registeredService);
            services.AddScoped<TInterface, TNewImplementation>();
        }
    }
}