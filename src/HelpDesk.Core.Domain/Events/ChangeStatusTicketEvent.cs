using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.Core.Domain.Events
{
    public sealed record ChangeStatusTicketEvent(int IdTicket, TicketStatuses ChangedStatus, int IdUserPerformedAction);
}
