using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ApiGateway.IntegrationTests.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        #region Extension Methods

        public static void RemoveDbContext<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<TDbContext>));
            if (descriptor is not null) services.Remove(descriptor);
        }

        #endregion
    }
}
