using System.Net;
using HelpDesk.ApiGateway.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace HelpDesk.ApiGateway.Extensions
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

        internal static IApplicationBuilder SkipCertificateValidation(this IApplicationBuilder builder)
        {
            ServicePointManager.ServerCertificateValidationCallback
                += (sender, certificate, chain, sslPolicyErrors) => true;

            return builder;
        }

        #endregion
    }
}
