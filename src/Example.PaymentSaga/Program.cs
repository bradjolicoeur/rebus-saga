using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Example.PaymentProcessor.Contracts.Commands;
using Example.PaymentProcessor.Contracts.Events;
using Example.PaymentSaga.Mapper;
using Example.PaymentSaga.Messages;
using Example.WebApp.Contracts.Messages;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutomapperProfile>());

        // Automatically register all handlers from the assembly of a given type
        services.AutoRegisterHandlersFromAssemblyOf<Program>();

        // Configure Rebus
        services.AddRebus(
            (configure, sp) => configure
                .Logging(l => l.MicrosoftExtensionsLogging(sp.GetRequiredService<ILoggerFactory>()))
                .Transport(t => t.UseRabbitMq("amqp://rabbitmq:rabbitmq@localhost", "example.paymentsaga"))
                //.Sagas(s => s.StoreInSqlServer(sagaDbConnectionString, "Sagas", "SagaIndex"))
                .Sagas(s => s.StoreInMemory())
                //.Timeouts(to => to.StoreInSqlServer(sagaDbConnectionString, "Timeouts"))
                .Timeouts(to => to.StoreInMemory())
                .Routing(r => r.TypeBased()
                    .MapAssemblyOf<MakePayment>("example.paymentprocessor")
                    .MapAssemblyOf<ProcessPaymentTimeout>("example.paymentsaga")
                    .MapAssemblyOf<ProcessPaymentReply>("example.webapp")),
            onCreated: async bus => await bus.Subscribe<CompletedMakePayment>());
    })
    .RunConsoleAsync();
