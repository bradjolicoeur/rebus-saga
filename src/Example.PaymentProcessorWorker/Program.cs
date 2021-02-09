using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.PaymentProcessorWorker
{
    internal sealed class Program
    {
        private static async Task Main(string[] args)
        {

            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddHostedService<ConsoleHostedService>();

                    // Automatically register all handlers from the assembly of a given type...
                    services.AutoRegisterHandlersFromAssemblyOf<Program>();

                    //Configure Rebus
                    services.AddRebus(configure => configure
                        .Logging(l => l.ColoredConsole())
                        .Transport(t => t.UseRabbitMq("amqp://rabbitmq:rabbitmq@localhost", "example.paymentprocessor"))
                        //.Routing(r => r.TypeBased()
                        //    .MapAssemblyOf<ProcessPayment>("example.paymentprocessor")
                        //    .MapAssemblyOf<ProcessPaymentTimeout>("example.paymentsaga"))

                        );


                })
                .RunConsoleAsync();
        }
    }


    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;


        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IServiceProvider serviceProvider,
            IBus bus)
        {
            _logger = logger;
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting Service");

            _serviceProvider.UseRebus();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }


    }
}
