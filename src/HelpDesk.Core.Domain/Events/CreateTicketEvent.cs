namespace HelpDesk.Core.Domain.Events
{    
    public sealed record CreateTicketEvent(int IdCategory, int IdUserRequester, string Description);
}
