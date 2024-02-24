namespace HelpDesk.ConsumerService.Api.Contracts
{
    public static class ApiRoutes
    {
        public static class Category
        {
            public const string Get = "categories";
            public const string GetById = "categories/{idCategory:int}";
        }

        public static class Tickets
        {
            public const string Get = "tickets";
            public const string GetById = "tickets/{idTicket:int}";
        }

        public static class TicketStatus
        {
            public const string Get = "ticketstatus";
            public const string GetById = "ticketstatus/{idTicketStatus:int}";
        }
    }
}
