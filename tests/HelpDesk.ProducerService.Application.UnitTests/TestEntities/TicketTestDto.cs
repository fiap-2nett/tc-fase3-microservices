using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Application.UnitTests.TestEntities
{
    internal class TicketTestDto : TicketDto
    {
        public TicketTestDto(int idTicket, CategoryDto category, string description, UserDto userRequester)
            : base(category, description, userRequester)
        {
            IdTicket = idTicket;
        }

        public static TicketTestDto Create(int idTicket, CategoryDto category, string description,
            UserDto userRequester, UserDto userAssigned = null, bool toComplete = false, bool toCancelled = false)
        {
            var ticket = new TicketTestDto(idTicket, category, description, userRequester);

            if (userAssigned is not null)
                ticket.AssignTo(userAssigned, userAssigned);

            if (toComplete)
                ticket.Complete(userAssigned);

            if (toCancelled)
                ticket.Cancel("Lorem ipsum dolor sit amet.", userAssigned);

            return ticket;
        }

        private void AssignTo(UserDto userAssigned, UserDto userPerformedAction)
        {
            IdUserAssigned = userAssigned.IdUser;
            LastUpdatedBy = userPerformedAction.IdUser;
            Status = new TicketStatusDto((byte)TicketStatuses.Assigned, default);
        }

        public void Cancel(string cancellationReason, UserDto userPerformedAction)
        {
            CancellationReason = cancellationReason;
            LastUpdatedBy = userPerformedAction.IdUser;
            Status = new TicketStatusDto((byte)TicketStatuses.Cancelled, default);
        }

        public void Complete(UserDto userPerformedAction)
        {
            LastUpdatedBy = userPerformedAction.IdUser;
            Status = new TicketStatusDto((byte)TicketStatuses.Completed, default);
        }
    }
}
