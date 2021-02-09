using Example.PaymentProcessor.Contracts.Commands;
using Example.PaymentProcessor.Contracts.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using System;
using System.Threading.Tasks;

namespace Example.PaymentProcessorWorker.Handlers
{
    class HandleMakePayment : IHandleMessages<MakePayment>
    {
        private readonly ILogger<HandleMakePayment> _logger;
        private readonly IBus _bus;

        public HandleMakePayment(ILogger<HandleMakePayment> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task Handle(MakePayment message)
        {

            _logger.LogInformation("Processing Payment " + message.ReferenceId);

            await _bus.Publish(
                new CompletedMakePayment
                {
                    ReferenceId = message.ReferenceId,
                    ConfirmationId = Guid.NewGuid().ToString(),
                    Status = "Approved",
                    StatusDate = DateTime.UtcNow,
                });
        }

        class CompletedMakePayment : ICompletedMakePayment
        {
            public string ReferenceId { get; set; }
            public string ConfirmationId { get; set; }
            public string Status { get; set; }
            public DateTime StatusDate { get; set; }
        }
    }
}
