using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class CancelTicketEventValidator
    {
        public static void ValidateAndThrow(TicketDto ticket, string cancellationReason, UserDto userPerformedAction,
            out CancelTicketEvent @event)
        {
            @event = default;

            if (userPerformedAction is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (userPerformedAction.UserRole == UserRoles.General && ticket.IdUserRequester != userPerformedAction.IdUser)
                throw new InvalidPermissionException(DomainErrors.Ticket.CannotBeCancelledByThisUser);

            if (userPerformedAction.UserRole == UserRoles.Analyst && ticket.IdUserAssigned != userPerformedAction.IdUser && ticket.IdUserRequester != userPerformedAction.IdUser)
                throw new InvalidPermissionException(DomainErrors.Ticket.CannotBeCancelledByThisUser);

            if (cancellationReason.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.Ticket.CancellationReasonIsRequired);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.Completed || ticket.Status.IdStatus == (byte)TicketStatuses.Cancelled)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            @event = new CancelTicketEvent(ticket.IdTicket, cancellationReason, userPerformedAction.IdUser);
        }
    }
}
