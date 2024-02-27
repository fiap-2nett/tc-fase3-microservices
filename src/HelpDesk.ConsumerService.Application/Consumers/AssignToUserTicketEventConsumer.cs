using System;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.Core.Domain.Events;
using MassTransit;

namespace HelpDesk.ConsumerService.Application.Consumers
{
    public sealed class AssignToUserTicketEventConsumer : IConsumer<AssignToUserTicketEvent>
    {
        #region Read-Only Fields

        private readonly ITicketService _ticketService;

        #endregion

        #region Constructors

        public AssignToUserTicketEventConsumer(ITicketService ticketService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        }

        #endregion

        public async Task Consume(ConsumeContext<AssignToUserTicketEvent> context)
        {            
            var @event = context.Message;
            await _ticketService.AssignToUserAsync(@event.IdTicket, @event.IdUserAssigned, @event.IdUserPerformedAction);      
        }
    }
}
