using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class UpdateTicketEventValidator
    {
        public static void ValidateAndThrow(TicketDto ticket, CategoryDto category, string description, UserDto userPerformedAction,
            out UpdateTicketEvent @event)
        {
            @event = default;

            if (category is null)
                throw new DomainException(DomainErrors.Category.NotFound);

            if (description.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.Ticket.DescriptionIsRequired);

            if (userPerformedAction is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (ticket.IdUserRequester != userPerformedAction.IdUser)
                throw new InvalidPermissionException(DomainErrors.User.InvalidPermissions);

            if (ticket.Status.IdStatus == (byte)TicketStatuses.Completed || ticket.Status.IdStatus == (byte)TicketStatuses.Cancelled)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            @event = new UpdateTicketEvent(ticket.IdTicket, category.IdCategory, description, userPerformedAction.IdUser);
        }
    }
}
