namespace HelpDesk.ProducerService.Infrastructure.Microservices
{
    public sealed class UserApiSettings
    {
        #region Constants

        public const string SettingsKey = "UserApiService";

        #endregion

        #region Properties

        public string Url { get; set; } = string.Empty;
        public int Timeout { get; set; } = 60;

        #endregion
    }
}
