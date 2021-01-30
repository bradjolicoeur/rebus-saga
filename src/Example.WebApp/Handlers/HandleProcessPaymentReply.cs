using Example.PaymentSaga.Contracts.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace Example.WebApp.Handlers
{
    public class HandleProcessPaymentReply : IHandleMessages<ProcessPaymentReply>
    {
        private readonly ILogger _log;
        public HandleProcessPaymentReply(ILogger<HandleProcessPaymentReply> logger)
        {
            _log = logger;
        }

        public Task Handle(ProcessPaymentReply message)
        {
            //This is where you might put a webhook post to the caller, write to SNS topic or push notification via signalr
            //This can address the situation where the immediate response was pending
            _log.LogInformation("Handled ProcessPaymentReply " + message.ReferenceId + " " + message.Status + " webhook post");

            return Task.CompletedTask;
        }
    }
}
