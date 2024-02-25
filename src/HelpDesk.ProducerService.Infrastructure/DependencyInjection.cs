using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Authentication;
using HelpDesk.Core.Domain.Authentication.Settings;
using HelpDesk.Core.Domain.Cryptography;
using HelpDesk.Core.Domain.MessageBroker.Settings;
using HelpDesk.ProducerService.Application.Core.Abstractions.EventBus;
using HelpDesk.ProducerService.Domain.Repositories;
using HelpDesk.ProducerService.Infrastructure.MessageBroker;
using HelpDesk.ProducerService.Infrastructure.Microservices;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.ProducerService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));
            services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

            services.Configure<UserApiSettings>(configuration.GetSection(UserApiSettings.SettingsKey));
            services.Configure<TicketApiSettings>(configuration.GetSection(TicketApiSettings.SettingsKey));
            services.Configure<CategoryApiSettings>(configuration.GetSection(CategoryApiSettings.SettingsKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]))
                    };
                });

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
                {
                    busFactoryConfigurator.Host(new Uri(configuration["MessageBroker:Host"]), host =>
                    {
                        host.Username(configuration["MessageBroker:Username"]);
                        host.Password(configuration["MessageBroker:Password"]);

                        busFactoryConfigurator.ConfigureEndpoints(context);
                    });
                });
            });

            services.AddScoped<IUserSessionProvider, UserSessionProvider>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddTransient<IEventBus, EventBus>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IPasswordHashChecker, PasswordHasher>();

            services
                .AddHttpClient(typeof(IUserRepository).FullName, client =>
                {
                    client.BaseAddress = new Uri(configuration["UserApiService:Url"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(double.Parse(configuration["UserApiService:Timeout"]));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
                });

            services
                .AddHttpClient(typeof(ICategoryRepository).FullName, client =>
                {
                    client.BaseAddress = new Uri(configuration["CategoryApiService:Url"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(double.Parse(configuration["CategoryApiService:Timeout"]));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
                });

            services
                .AddHttpClient(typeof(ITicketRepository).FullName, client =>
                {
                    client.BaseAddress = new Uri(configuration["TicketApiService:Url"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(double.Parse(configuration["TicketApiService:Timeout"]));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
                });

            return services;
        }
    }
}
