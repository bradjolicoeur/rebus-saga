using Example.PaymentSaga.Contracts.Commands;
using Example.WebApp.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;

namespace Example.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register handlers 
            services.AutoRegisterHandlersFromAssemblyOf<HandleProcessPaymentReply>();


            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Options(o =>
                {
                    o.EnableSynchronousRequestReply();
                    o.SetNumberOfWorkers(1);
                    o.SetMaxParallelism(1);
                })
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "example.webapp"))
                .Routing(r => r.TypeBased().MapAssemblyOf<ProcessPayment>("example.paymentsaga")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus();
            //or optionally act on the bus
            //app.ApplicationServices.UseRebus(async bus => await bus.Subscribe<Message1>());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
