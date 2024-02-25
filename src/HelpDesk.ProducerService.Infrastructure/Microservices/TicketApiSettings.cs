namespace HelpDesk.ProducerService.Infrastructure.Microservices
{
    public sealed class TicketApiSettings
    {
        #region Constants

        public const string SettingsKey = "TicketApiService";

        #endregion

        #region Properties

        public string Url { get; set; } = string.Empty;
        public int Timeout { get; set; } = 60;

        #endregion
    }
}
