using HelpDesk.ApiGateway.Application.Core.Abstractions.Services;
using HelpDesk.ApiGateway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ApiGateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
