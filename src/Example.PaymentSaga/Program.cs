using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Example.PaymentSaga.Contracts.Commands;
using Microsoft.Extensions.Logging;
using System.Threading;
using Example.PaymentProcessor.Contracts.Commands;
using AutoMapper;
using Example.PaymentSaga.Mapper;
using Example.PaymentSaga.Messages;
using Example.WebApp.Contracts.Messages;
using Example.PaymentProcessor.Contracts.Events;

namespace Example.PaymentSaga
{
    internal sealed class Program
    {
        private static async Task Main(string[] args)
        {
            var sagaDbConnectionString = "Data Source=.;Initial Catalog=rebus-saga;Integrated Security=True";

            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAutoMapper(typeof(AutomapperProfile));

                    services.AddHostedService<ConsoleHostedService>();

                    // Automatically register all handlers from the assembly of a given type...
                    services.AutoRegisterHandlersFromAssemblyOf<Program>();

                    //Configure Rebus
                    services.AddRebus(configure => configure
                        .Logging(l => l.ColoredConsole())
                        .Transport(t => t.UseRabbitMq("amqp://rabbitmq:rabbitmq@localhost", "example.paymentsaga"))
                        .Sagas(s => s.StoreInSqlServer(sagaDbConnectionString, "Sagas", "SagaIndex"))
                        .Timeouts(to => to.StoreInSqlServer(sagaDbConnectionString,"Timeouts"))
                        .Routing(r => r.TypeBased()
                            .MapAssemblyOf<MakePayment>("example.paymentprocessor")
                            .MapAssemblyOf<ProcessPaymentTimeout>("example.paymentsaga")
                            .MapAssemblyOf<ProcessPaymentReply>("example.webapp"))

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

            _serviceProvider.UseRebus(async bus => await bus.Subscribe<ICompletedMakePayment>());    


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }

       
    }
}
