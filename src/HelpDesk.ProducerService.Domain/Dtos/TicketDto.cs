using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;

namespace HelpDesk.ProducerService.Domain.Dtos
{
    public class TicketDto
    {
        #region Properties

        public int IdTicket { get; set; }
        public int IdUserRequester { get; set; }
        public int? IdUserAssigned { get; set; }
        public string Description { get; set; }
        public CategoryDto Category { get; set; }
        public TicketStatusDto Status { get; set; }
        public string CancellationReason { get; set; }

        public int? LastUpdatedBy { get; set; }

        #endregion

        #region Constructors

        public TicketDto()
        { }

        public TicketDto(CategoryDto category, string description, UserDto userRequester)
        {
            
            if (category is null)
                throw new DomainException(DomainErrors.Category.NotFound);

            if (userRequester is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (description.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.Ticket.DescriptionIsRequired);

            Category = category;
            Description = description;
            IdUserRequester = userRequester.IdUser;
            Status = new TicketStatusDto((byte)TicketStatuses.New, default);            
        }

        #endregion
    }
}
