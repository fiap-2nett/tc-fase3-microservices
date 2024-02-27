using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class CompleteTicketEventValidator
    {
        public static void ValidateAndThrow(TicketDto ticket, UserDto userPerformedAction,
            out CompleteTicketEvent @event)
        {
            @event = default;

            if (userPerformedAction is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (userPerformedAction.UserRole == UserRoles.General)
                throw new InvalidPermissionException(DomainErrors.Ticket.CannotBeCompletedByThisUser);

            if (userPerformedAction.UserRole == UserRoles.Analyst && ticket.IdUserAssigned != userPerformedAction.IdUser)
                throw new InvalidPermissionException(DomainErrors.Ticket.CannotBeCompletedByThisUser);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.New)
                throw new DomainException(DomainErrors.Ticket.HasNotBeenAssignedToAUser);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.Completed || ticket.Status.IdStatus == (byte)TicketStatuses.Cancelled)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            @event = new CompleteTicketEvent(ticket.IdTicket, userPerformedAction.IdUser);
        }
    }
}
