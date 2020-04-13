using System;
using Xunit;

namespace Glue.Delivery.DeliveryState.Worker.Test
{
    public class ProgramTests
    {
        [Fact]
        public async void Program_Should_Load_Dependencies_Without_Error()
        {
            Environment.SetEnvironmentVariable("Environment","Development");
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID","abc");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY","def");
            Environment.SetEnvironmentVariable("ScheduledService__Interval","00:01:00");

            var hostBuilder = Program.BuildHost(new string[] { });
            
            Assert.NotNull(hostBuilder);

            var host = hostBuilder.Build();
            
            Assert.NotNull(host);

        }
    }
}