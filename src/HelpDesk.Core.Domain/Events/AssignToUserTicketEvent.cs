namespace HelpDesk.Core.Domain.Events
{
    public sealed record AssignToUserTicketEvent(int IdTicket, int IdUserAssigned, int IdUserPerformedAction);
}
