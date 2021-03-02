using AutoMapper;
using Example.PaymentProcessor.Contracts.Commands;
using Example.PaymentProcessor.Contracts.Events;
using Example.PaymentSaga.Contracts.Commands;
using Example.PaymentSaga.Messages;
using Example.PaymentSaga.Models;
using Example.WebApp.Contracts.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Sagas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.PaymentSaga.Sagas
{
    class ProcessPaymentSaga : Saga<ProcessPaymentData>,
        IAmInitiatedBy<ProcessPayment>,
        IHandleMessages<CompletedMakePayment>,
        IHandleMessages<ProcessPaymentTimeout>
    {
        private readonly ILogger<ProcessPaymentSaga> _logger;

        private readonly IMapper _mapper;
        private readonly IBus _bus;

        public ProcessPaymentSaga(ILogger<ProcessPaymentSaga> logger, IBus bus, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _bus = bus;
        }

        public async Task Handle(ProcessPayment message)
        {
            
            _logger.LogInformation("Start Saga for " + Data.ReferenceId);

            _logger.LogInformation("Account:" + message.AccountNumberEncrypted);

            //update saga data
            _mapper.Map(message, Data);
            Data.Originator = MessageContext.Current.Headers[Headers.ReturnAddress];
            Data.OriginatorId = MessageContext.Current.Headers[Headers.CorrelationId];

            //Send command
            await _bus.Send(_mapper.Map<MakePayment>(Data));

            //Set timeout
            await _bus.DeferLocal(TimeSpan.FromSeconds(3), new ProcessPaymentTimeout { ReferenceId = Data.ReferenceId });

            var reply = _mapper.Map<ProcessPaymentReply>(Data);
            reply.Status = "Pending";
            reply.StatusDate = DateTime.UtcNow;
            reply.ConfirmationId = MessageContext.Current.Headers[Headers.MessageId];

        }

        public async Task Handle(CompletedMakePayment message)
        {
            _logger.LogInformation("Handle ICompletedMakePayment " + message.ReferenceId);

            //update saga
            _mapper.Map(message, Data);

            await _bus.Advanced.Routing.Send(Data.Originator,_mapper.Map<ProcessPaymentReply>(Data), 
                    new Dictionary<string, string> { 
                        { Headers.InReplyTo, Data.OriginatorId}
                    });
        }


        public async Task Handle(ProcessPaymentTimeout state)
        {

            if (String.IsNullOrEmpty(Data.Status))
            {
                _logger.LogInformation("Handle Timeout for " + Data.ReferenceId);

                var reply = _mapper.Map<ProcessPaymentReply>(Data);
                reply.Status = "Pending";
                reply.StatusDate = DateTime.UtcNow;

                _logger.LogInformation("correlationid" + Data.Id);

                await _bus.Advanced.Routing.Send(Data.Originator, reply,
                        new Dictionary<string, string> {
                             { Headers.InReplyTo, Data.OriginatorId}
                        });
            }

        }

        protected override void CorrelateMessages(ICorrelationConfig<ProcessPaymentData> config)
        {
            config.Correlate<ProcessPayment>(m => m.ReferenceId, d => d.ReferenceId);
            config.Correlate<CompletedMakePayment>(m => m.ReferenceId, d => d.ReferenceId);
            config.Correlate<ProcessPaymentTimeout>(m => m.ReferenceId, d => d.ReferenceId);
        }

    }
}
