using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.ConsumerService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ConsumerService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {            
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITicketStatusService, TicketStatusService>();

            return services;
        }
    }
}
