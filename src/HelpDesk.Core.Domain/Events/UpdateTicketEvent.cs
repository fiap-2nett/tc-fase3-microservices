namespace HelpDesk.Core.Domain.Events
{
    public sealed record UpdateTicketEvent(int IdTicket, int IdCategory, string Description, int IdUserPerformedAction);
}
