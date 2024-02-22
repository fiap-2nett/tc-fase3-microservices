using HelpDesk.Core.Domain.Entities;

namespace HelpDesk.Core.Domain.Authentication
{
    public interface IJwtProvider
    {
        #region IJwtProvider Members

        string Create(User user);

        #endregion
    }
}
