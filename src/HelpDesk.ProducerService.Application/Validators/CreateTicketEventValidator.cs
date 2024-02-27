using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.Validators
{
    internal sealed class CreateTicketEventValidator
    {
        public static void ValidateAndThrow(CategoryDto category, UserDto userRequester, string description,
            out CreateTicketEvent @event)
        {
            @event = default;

            if (category is null)
                throw new DomainException(DomainErrors.Category.NotFound);

            if (userRequester is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (description.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.Ticket.DescriptionIsRequired);

            @event = new CreateTicketEvent(category.IdCategory, userRequester.IdUser, description);
        }
    }
}
