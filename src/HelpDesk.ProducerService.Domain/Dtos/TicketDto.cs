using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.ProducerService.Domain.Dtos
{
    public sealed record TicketDto(int IdTicket, int IdCategory, string Description, int IdUserRequester, int? IdUserAssigned = null,
        TicketStatuses TicketStatus = TicketStatuses.New);
}
