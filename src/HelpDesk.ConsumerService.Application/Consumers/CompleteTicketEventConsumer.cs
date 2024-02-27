using System;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.Core.Domain.Events;
using MassTransit;

namespace HelpDesk.ConsumerService.Application.Consumers
{
    public sealed class CompleteTicketEventConsumer : IConsumer<CompleteTicketEvent>
    {
        #region Read-Only Fields

        private readonly ITicketService _ticketService;

        #endregion

        #region Constructors

        public CompleteTicketEventConsumer(ITicketService ticketService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        }

        #endregion

        public async Task Consume(ConsumeContext<CompleteTicketEvent> context)
        {            
            var @event = context.Message;
            await _ticketService.CompleteAsync(@event.IdTicket, @event.IdUserPerformedAction);      
        }
    }
}
