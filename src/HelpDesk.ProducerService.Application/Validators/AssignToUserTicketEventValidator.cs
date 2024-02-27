using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class AssignToUserTicketEventValidator
    {
        public static void ValidateAndThrow(TicketDto ticket, UserDto userAssigned, UserDto userPerformedAction,
            out AssignToUserTicketEvent @event)
        {
            @event = default;

            if (userAssigned is null || userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            if (userAssigned.UserRole != UserRoles.Analyst)
                throw new InvalidPermissionException(DomainErrors.Ticket.CannotBeAssignedToThisUser);

            if (userPerformedAction.UserRole == UserRoles.General)
                throw new InvalidPermissionException(DomainErrors.User.InvalidPermissions);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.Completed || ticket.Status.IdStatus == (byte)TicketStatuses.Cancelled)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.New && userPerformedAction.IdUser != userAssigned.IdUser && userPerformedAction.UserRole != UserRoles.Administrator)
                throw new InvalidPermissionException(DomainErrors.User.InvalidPermissions);

            if (ticket.IdUserAssigned != userPerformedAction.IdUser && userPerformedAction.UserRole != UserRoles.Administrator && ticket.Status.IdStatus != (byte)TicketStatuses.New)
                throw new InvalidPermissionException(DomainErrors.User.InvalidPermissions);

            @event = new AssignToUserTicketEvent(ticket.IdTicket, userAssigned.IdUser, userPerformedAction.IdUser);
        }
    }
}
