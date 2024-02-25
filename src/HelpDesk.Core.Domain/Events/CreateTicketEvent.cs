namespace HelpDesk.Core.Domain.Events
{
    public sealed class CreateTicketEvent
    {
        public int IdCategory { get; set; }
        public int IdUserRequester { get; set; }
        public string Description { get; set; }
    };
}
