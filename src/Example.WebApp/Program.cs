using Example.PaymentSaga.Contracts.Commands;
using Example.WebApp.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register handlers
builder.Services.AutoRegisterHandlersFromAssemblyOf<HandleProcessPaymentReply>();

// Configure and register Rebus
builder.Services.AddRebus((configure, sp) => configure
    .Options(o => o.EnableSynchronousRequestReply())
    .Logging(l => l.MicrosoftExtensionsLogging(sp.GetRequiredService<ILoggerFactory>()))
    .Transport(t => t.UseRabbitMq("amqp://rabbitmq:rabbitmq@localhost", "example.webapp"))
    .Routing(r => r.TypeBased().MapAssemblyOf<ProcessPayment>("example.paymentsaga")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
