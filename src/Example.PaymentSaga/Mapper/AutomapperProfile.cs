using AutoMapper;
using Example.PaymentProcessor.Contracts.Commands;
using Example.PaymentProcessor.Contracts.Events;
using Example.PaymentSaga.Contracts.Commands;
using Example.PaymentSaga.Models;
using Example.WebApp.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.PaymentSaga.Mapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<ProcessPayment, MakePayment>();
            CreateMap<ProcessPayment, ProcessPaymentData>()
                .ForMember(dest => dest.ReferenceId, act => act.Ignore()); //Saga will take care of this
            CreateMap<ICompletedMakePayment, ProcessPaymentData>()
                .ForMember(dest => dest.ReferenceId, act => act.Ignore()); //Saga will take care of this

            CreateMap<ProcessPaymentData, MakePayment>();
            CreateMap<ProcessPaymentData, ProcessPaymentReply>();
        }
    }
}
