using Example.PaymentSaga.Contracts.Commands;
using Example.PaymentSaga.Contracts.Messages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rebus;
using Rebus.Bus;
using System;
using System.Threading.Tasks;

namespace Example.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessPaymentController : ControllerBase
    {
        IBus messageSession;

        public ProcessPaymentController(IBus messageSession)
        {
            this.messageSession = messageSession;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var message = new ProcessPayment { ReferenceId = Guid.NewGuid().ToString(), AccountNumberEncrypted = "123456", RoutingNumber = "555555", Amount = 100.45M, RequestDate = DateTime.UtcNow };
            var response = await messageSession.SendRequest<ProcessPaymentReply>(message);
            return "Payment Status: " + JsonConvert.SerializeObject(response);
        }
    }
}
