using BuildingBlocks.Exceptions.Handler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Exceptions.Extensions;
public static class ExceptionHandlingExtensions
{
    private const string CorrelationHeader = "X-Correlation-ID";

    public static IServiceCollection AddBuildingBlocksExceptionHandling(this IServiceCollection services, IHostEnvironment env)
    {
        // 1) Writer/Service padrão de ProblemDetails + customização global
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                // Sempre: traceId
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;

                // Sempre: correlationId (header ou traceId)
                var correlationId =
                    ctx.HttpContext.Request.Headers.TryGetValue(CorrelationHeader, out var cid) &&
                    !string.IsNullOrWhiteSpace(cid)
                        ? cid.ToString()
                        : ctx.HttpContext.TraceIdentifier;

                ctx.ProblemDetails.Extensions["correlationId"] = correlationId;

                // Opcional: ecoa correlation id na resposta
                ctx.HttpContext.Response.Headers[CorrelationHeader] = correlationId;

                // Sempre: timestamp
                ctx.ProblemDetails.Extensions["timestampUtc"] = DateTime.UtcNow.ToString("O");

                // Somente DEV: detalhes técnicos (stack trace etc.)
                if (env.IsDevelopment() && ctx.Exception is not null)
                {
                    ctx.ProblemDetails.Extensions["exception"] = new
                    {
                        type = ctx.Exception.GetType().FullName,
                        message = ctx.Exception.Message,
                        stackTrace = ctx.Exception.ToString()
                    };
                }
            };
        });

        // 2) Handler moderno de exceções (IExceptionHandler)
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static IApplicationBuilder UseBuildingBlocksExceptionHandling(this IApplicationBuilder app)
    {
        // O middleware que dispara o fluxo de tratamento de exceções
        app.UseExceptionHandler(_ => { }); // delegate vazio é ok com IExceptionHandler registrado
        return app;
    }

}
