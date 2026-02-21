using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.ServiceProvider;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Automatically register all handlers from the assembly of a given type
        services.AutoRegisterHandlersFromAssemblyOf<Program>();

        // Configure Rebus
        services.AddRebus((configure, sp) => configure
            .Logging(l => l.MicrosoftExtensionsLogging(sp.GetRequiredService<ILoggerFactory>()))
            .Transport(t => t.UseRabbitMq("amqp://rabbitmq:rabbitmq@localhost", "example.paymentprocessor")));
    })
    .RunConsoleAsync();
