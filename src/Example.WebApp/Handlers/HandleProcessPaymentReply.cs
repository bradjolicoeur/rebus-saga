using Example.WebApp.Contracts.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Pipeline;
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

            //_log.LogInformation("ReplyTo " + MessageContext.Current.Headers[Headers.InReplyTo]);

            return Task.CompletedTask;
        }
    }
}
