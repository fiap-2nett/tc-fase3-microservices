using HelpDesk.ConsumerService.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace HelpDesk.ConsumerService.Api.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        #region Extension Methods

        internal static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder builder)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(swaggerUiOptions => swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

            return builder;
        }

        internal static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionHandlerMiddleware>();

        #endregion
    }
}
