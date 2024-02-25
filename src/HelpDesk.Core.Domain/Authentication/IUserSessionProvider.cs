namespace HelpDesk.Core.Domain.Authentication
{
    public interface IUserSessionProvider
    {
        #region IUserSessionProvider Members

        int IdUser { get; }
        string Authorization { get; }

        #endregion
    }
}
