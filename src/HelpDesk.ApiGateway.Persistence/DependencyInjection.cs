using HelpDesk.ApiGateway.Application.Core.Abstractions.Data;
using HelpDesk.ApiGateway.Domain.Repositories;
using HelpDesk.ApiGateway.Persistence.Infrastructure;
using HelpDesk.ApiGateway.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ApiGateway.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(ConnectionString.SettingsKey);

            services.AddSingleton(new ConnectionString(connectionString));
            services.AddDbContext<EFContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<EFContext>());
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<EFContext>());

            services.AddScoped<IUserRepository, UserRepository>();            

            return services;
        }
    }
}
