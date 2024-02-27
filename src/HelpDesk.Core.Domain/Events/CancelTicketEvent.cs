namespace HelpDesk.Core.Domain.Events
{
    public sealed record CancelTicketEvent(int IdTicket, string CancellationReason, int IdUserPerformedAction);
}
