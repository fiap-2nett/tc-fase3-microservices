using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class ChangeStatusTicketEventValidator
    {
        public static void ValidateAndThrow(TicketDto ticket, TicketStatuses changedStatus, UserDto userPerformedAction,
            out ChangeStatusTicketEvent @event)
        {
            @event = default;

            if (userPerformedAction is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (userPerformedAction.UserRole == UserRoles.General || userPerformedAction.UserRole == UserRoles.Analyst && ticket.IdUserAssigned != userPerformedAction.IdUser)
                throw new InvalidPermissionException(DomainErrors.Ticket.StatusCannotBeChangedByThisUser);

            if (changedStatus == TicketStatuses.New)
                throw new DomainException(DomainErrors.Ticket.CannotChangeStatusToNew);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.Cancelled || ticket.Status.IdStatus == (byte)TicketStatuses.Completed)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            if (changedStatus != TicketStatuses.InProgress && changedStatus != TicketStatuses.OnHold && changedStatus != TicketStatuses.Completed)
                throw new DomainException(DomainErrors.Ticket.StatusNotAllowed);

            @event = new ChangeStatusTicketEvent(ticket.IdTicket, changedStatus, userPerformedAction.IdUser);
        }
    }
}
