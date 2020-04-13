using System;
using System.Threading;
using System.Threading.Tasks;
using Glue.Delivery.Core.Configuration;
using Glue.Delivery.Core.Models;
using Glue.Delivery.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Glue.Delivery.DeliveryState.Worker.Test
{
    public class StateUpdateHostedServiceTests
    {
        private readonly StateUpdateHostedService _hostedService;
        private readonly Mock<IDeliveryStateService> _deliveryStateService;
        
        public StateUpdateHostedServiceTests()
        {
            _deliveryStateService = new Mock<IDeliveryStateService>();
            var mockLogger = new Mock<ILogger<StateUpdateHostedService>>();
            
            _hostedService = new StateUpdateHostedService(mockLogger.Object, _deliveryStateService.Object, new ScheduledServiceConfiguration{ Interval = TimeSpan.FromSeconds(3)});

            _deliveryStateService.Setup(x => x.ExpireDeliveries(It.IsAny<DateTime>()))
                .ReturnsAsync(ServiceResult.Succeeded);
        }

        [Fact]
        public async void HostedService_Should_Trigger_ExpireDeliveries_On_Service_On_Timer()
        {
            await _hostedService.StartAsync(CancellationToken.None);
            
            var initialDelay = Task.Delay(1000);
            await initialDelay;
            
            _deliveryStateService.Verify(x => x.ExpireDeliveries(It.IsAny<DateTime>()), Times.Once);

            var delay = Task.Delay(3000);
            await delay;
            
            _deliveryStateService.Verify(x => x.ExpireDeliveries(It.IsAny<DateTime>()), Times.Exactly(2));

        }
    }
}