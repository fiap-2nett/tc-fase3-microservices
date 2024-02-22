using HelpDesk.ProducerService.Application.Core.Abstractions.Data;
using HelpDesk.ProducerService.Domain.Repositories;
using HelpDesk.ProducerService.Persistence.Infrastructure;
using HelpDesk.ProducerService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.ProducerService.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
        {


            return services;
        }
    }
}
