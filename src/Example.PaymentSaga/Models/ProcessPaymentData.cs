using Rebus.Sagas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.PaymentSaga.Models
{
    class ProcessPaymentData : ISagaData
    {
        public Guid Id { get; set; }
        public int Revision { get; set; }


        public string ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumberEncrypted { get; set; }
        public string RoutingNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string ConfirmationId { get; set; }
        public string Originator { get; set; }
        public string OriginatorId { get; set; }
    }
}
