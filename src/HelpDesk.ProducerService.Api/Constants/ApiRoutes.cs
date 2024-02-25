namespace HelpDesk.ProducerService.Api.Constants
{
    public static class ApiRoutes
    {
        public static class Tickets
        {
            public const string Get = "tickets";
            public const string GetById = "tickets/{idTicket:int}";
            public const string Create = "tickets";
            public const string AssignTo = "tickets/{idTicket:int}/assign-to";
            public const string AssignToMe = "tickets/{idTicket:int}/assign-to/me";
            public const string Complete = "tickets/{idTicket:int}/complete";
            public const string ChangeStatus = "tickets/{idTicket:int}/change-status";
            public const string Update = "tickets/{idTicket:int}";
            public const string Cancel = "tickets/{idTicket:int}/cancel";
        }
    }
}
