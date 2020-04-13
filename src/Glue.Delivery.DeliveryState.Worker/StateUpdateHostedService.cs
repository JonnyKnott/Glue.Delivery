using System;
using System.Threading;
using System.Threading.Tasks;
using Glue.Delivery.Core.Configuration;
using Glue.Delivery.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glue.Delivery.DeliveryState.Worker
{
    public class StateUpdateHostedService : IHostedService
    {
        private readonly ILogger<StateUpdateHostedService> _logger;
        private readonly IDeliveryStateService _deliveryStateService;
        private readonly ScheduledServiceConfiguration _configuration;

        private Timer _timer;

        public StateUpdateHostedService(ILogger<StateUpdateHostedService> logger, IDeliveryStateService deliveryStateService, ScheduledServiceConfiguration configuration)
        {
            _logger = logger;
            _deliveryStateService = deliveryStateService;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExpireDeliveries, null, TimeSpan.Zero, _configuration.Interval);
            
            return Task.CompletedTask;
        }

        private async void ExpireDeliveries(object state)
        {
            _logger.LogInformation($"Checking for expired deliveries..");
            
            var result = await _deliveryStateService.ExpireDeliveries(DateTime.UtcNow);

            if (result.Success) return;
            
            foreach (var error in result.Errors)
                _logger.LogError(error);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            
            return Task.CompletedTask;
        }
    }
}