using System.Threading.Tasks;
using FluentAssertions;
using HelpDesk.ApiGateway.Application.Services;
using HelpDesk.ApiGateway.Application.UnitTests.TestEntities;
using HelpDesk.ApiGateway.Domain.Repositories;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Authentication;
using HelpDesk.Core.Domain.Authentication.Settings;
using HelpDesk.Core.Domain.Cryptography;
using HelpDesk.Core.Domain.Entities;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HelpDesk.ApiGateway.Application.UnitTests.Scenarios
{
    public sealed class AuthenticationServiceTests
    {
        #region Read-Only Fields

        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHashChecker _passwordHashChecker;

        private readonly Mock<IUserRepository> _userRepositoryMock;

        #endregion

        #region Constructors

        public AuthenticationServiceTests()
        {
            _userRepositoryMock = new();

            _jwtProvider = new JwtProvider(JwtOptions);
            _passwordHashChecker = new PasswordHasher();
        }

        #endregion

        #region Unit Tests

        #region Login

        [Fact]
        public async Task Login_Should_ReturnTokenResponseAsync_WithValidCredentials()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync(GetUser());

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var testResult = await authenticationService.Login("john.doe@test.com", @"John@123");

            // Assert
            testResult.Should().NotBeNull();
            testResult.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_Should_ThrowDomainException_WithInvalidEmail()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync((User)default);

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var action = () => authenticationService.Login("johndoe@test.com", @"John@123");

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Authentication.InvalidEmailOrPassword.Message);
        }

        [Fact]
        public async Task Login_Should_ThrowDomainException_WithInvalidPassword()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync(GetUser());

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var action = () => authenticationService.Login("john.doe@test.com", @"John@123456");

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Authentication.InvalidEmailOrPassword.Message);
        }

        #endregion

        #endregion

        #region Private Methods

        private UserTest GetUser() => new UserTest
        (
            idUser: 1,
            name: "John",
            surname: "Doe",
            email: Email.Create("john.doe@test.com"),
            userRole: UserRoles.Administrator,
            passwordHash: new PasswordHasher().HashPassword(Password.Create("John@123"))
        );

        private IOptions<JwtSettings> JwtOptions => Options.Create(new JwtSettings
        {
            Issuer = "http://localhost",
            Audience = "http://localhost",
            SecurityKey = "f143bfc760543ec317abd4e8748d9f2b44dfb07a",
            TokenExpirationInMinutes = 60
        });

        #endregion
    }
}
