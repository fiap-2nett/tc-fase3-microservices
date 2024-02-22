using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HelpDesk.Core.Domain.Authentication
{
    public sealed class UserSessionProvider : IUserSessionProvider
    {
        #region IUserSessionProvider Members

        public int IdUser { get; }

        #endregion

        #region Constructors

        public UserSessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            if (!int.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var idUser))
                throw new ArgumentException("The user identifier claim is required.", nameof(httpContextAccessor));

            IdUser = idUser;
        }

        #endregion
    }
}
