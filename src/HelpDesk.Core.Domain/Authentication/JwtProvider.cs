using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.Core.Domain.Authentication.Settings;
using HelpDesk.Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.Core.Domain.Authentication
{
    public sealed class JwtProvider : IJwtProvider
    {
        #region Read-Only Fields

        private readonly JwtSettings _jwtSettings;

        #endregion

        #region Constructors

        public JwtProvider(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        #endregion

        #region IJwtProvider Members

        public string Create(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name.ToString()),
                new Claim(ClaimTypes.Surname, user.Surname.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString())
            };

            var tokenExpirationTime = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                userClaims,
                null,
                tokenExpirationTime,
                signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
