using System;

namespace Example.WebApp.Contracts.Messages
{
    public class ProcessPaymentReply
    {
        public string ReferenceId { get; set; }
        public string ConfirmationId { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
