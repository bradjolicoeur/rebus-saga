using System;
using System.Collections.Generic;
using System.Text;

namespace Example.PaymentSaga.Messages
{
    class ProcessPaymentTimeout
    {
        public string ReferenceId { get; set; }
    }
}
