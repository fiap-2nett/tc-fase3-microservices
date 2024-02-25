using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HelpDesk.Core.Domain.Authentication;
using HelpDesk.ProducerService.Domain.Dtos;
using HelpDesk.ProducerService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace HelpDesk.ProducerService.Persistence.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        #region Read-Only Fields

        private readonly ILogger _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserSessionProvider _userSessionProvider;

        #endregion

        #region Constructors

        public UserRepository(ILogger<UserRepository> logger,
            IHttpClientFactory clientFactory, IUserSessionProvider userSessionProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _userSessionProvider = userSessionProvider ?? throw new ArgumentNullException(nameof(userSessionProvider));
        }

        #endregion

        #region IUserRepository Members

        public async Task<UserDto> GetByIdAsync(int idUser)
        {
            try
            {
                using (var httpClient = _clientFactory.CreateClient(typeof(IUserRepository).FullName))
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Authorization, _userSessionProvider.Authorization);

                    var response = await httpClient.GetAsync($"{idUser}");
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadFromJsonAsync<UserDto>();                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return default;
        }

        #endregion
    }
}
