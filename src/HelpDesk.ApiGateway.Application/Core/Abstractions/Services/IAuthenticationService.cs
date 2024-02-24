using System.Threading.Tasks;
using HelpDesk.ApiGateway.Application.Contracts.Authentication;

namespace HelpDesk.ApiGateway.Application.Core.Abstractions.Services
{
    public interface IAuthenticationService
    {
        #region IAuthenticationService Members

        Task<TokenResponse> Login(string email, string password);

        #endregion
    }
}
