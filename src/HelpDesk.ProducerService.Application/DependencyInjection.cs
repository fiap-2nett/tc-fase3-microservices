using HelpDesk.ProducerService.Application.Core.Abstractions.Services;
using HelpDesk.ProducerService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ProducerService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITicketService, TicketService>();

            return services;
        }
    }
}
