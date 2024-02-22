using System;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Contracts.Authentication;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.ConsumerService.Domain.Repositories;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Authentication;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.ValueObjects;

namespace HelpDesk.ConsumerService.Application.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        #region Read-Only Fields

        private readonly IJwtProvider _jwtProvider;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashChecker _passwordHashChecker;

        #endregion

        #region Constructors

        public AuthenticationService(IJwtProvider jwtProvider, IUserRepository userRepository, IPasswordHashChecker passwordHashChecker)
        {
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashChecker = passwordHashChecker ?? throw new ArgumentNullException(nameof(passwordHashChecker));
        }

        #endregion

        #region IAuthenticationService Members

        public async Task<TokenResponse> Login(string email, string password)
        {
            var emailResult = Email.Create(email);

            var user = await _userRepository.GetByEmailAsync(emailResult);
            if (user is null)
                throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);

            var passwordValid = user.VerifyPasswordHash(password, _passwordHashChecker);
            if (!passwordValid)
                throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);

            return new TokenResponse(_jwtProvider.Create(user));
        }

        #endregion
    }
}
