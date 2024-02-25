namespace HelpDesk.Core.Domain.MessageBroker.Settings
{
    public sealed record MessageBrokerSettings(string Host, string Username, string Password)
    {
        #region Constants

        public const string SettingsKey = "MessageBroker";

        #endregion
    }
}
