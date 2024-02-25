using HelpDesk.ProducerService.Api.Extensions;
using HelpDesk.ProducerService.Application;
using HelpDesk.ProducerService.Infrastructure;
using HelpDesk.ProducerService.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelpDesk.ProducerService.Api
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

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.ConfigureSwagger();
            }
            
            app.UseCustomExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
