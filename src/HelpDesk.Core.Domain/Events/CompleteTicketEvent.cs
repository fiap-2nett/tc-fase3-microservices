namespace HelpDesk.Core.Domain.Events
{
    public sealed record CompleteTicketEvent(int IdTicket, int IdUserPerformedAction);
}
