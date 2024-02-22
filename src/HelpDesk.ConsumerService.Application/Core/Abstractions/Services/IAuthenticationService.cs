using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Contracts.Authentication;

namespace HelpDesk.ConsumerService.Application.Core.Abstractions.Services
{
    public interface IAuthenticationService
    {
        #region IAuthenticationService Members

        Task<TokenResponse> Login(string email, string password);

        #endregion
    }
}
