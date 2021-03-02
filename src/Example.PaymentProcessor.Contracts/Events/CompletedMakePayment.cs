using System;

namespace Example.PaymentProcessor.Contracts.Events
{
    public class CompletedMakePayment 
    {
        public string ReferenceId { get; set; }
        public string ConfirmationId { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
