using HelpDesk.ApiGateway.Extensions;
using HelpDesk.ApiGateway.Application;
using HelpDesk.ApiGateway.Infrastructure;
using HelpDesk.ApiGateway.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net.Http;
using System.Net;
using Ocelot.Values;

namespace HelpDesk.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddPersistence(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            builder.Configuration
                   .AddJsonFile("SwaggerEndPoints.json", optional: false, reloadOnChange: true)
                   .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddSwaggerForOcelot(builder.Configuration);


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.ConfigureSwagger();
            }

            app.EnsureDatabaseCreated();
            app.UseCustomExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(x => x.MapControllers());
            app.UseOcelot();

            app.Run();
        }
    }
}
