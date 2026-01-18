using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler
{
    public sealed class CustomExceptionHandler(IProblemDetailsService problemDetailsService, IHostEnvironment env, ILogger<CustomExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (context.Response.HasStarted)
                return false;

            // Log with stack trace
            logger.LogError(exception,
                "Unhandled exception: {ExceptionType} - {Message}",
                exception.GetType().FullName,
                exception.Message);

            var mapped = Map(exception, env);

            context.Response.StatusCode = mapped.Status;

            // ValidationException (FluentValidation) -> ValidationProblemDetails com shape "errors"
            if (exception is ValidationException ve)
            {
                var errors = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var vpd = new ValidationProblemDetails(errors)
                {
                    Status = mapped.Status,
                    Title = mapped.Title,
                    Type = mapped.Type,
                    Detail = mapped.Detail,
                    Instance = context.Request.Path
                };

                // Extensions de "contrato" (não confundir com traceId/correlationId que vêm do AddProblemDetails)
                if (!string.IsNullOrWhiteSpace(mapped.Code))
                    vpd.Extensions["code"] = mapped.Code;

                if (!string.IsNullOrWhiteSpace(mapped.ExtraDetails))
                    vpd.Extensions["details"] = mapped.ExtraDetails;

                return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = vpd,
                    Exception = exception
                });
            }

            // Demais exceções -> ProblemDetails normal
            var pd = new ProblemDetails
            {
                Status = mapped.Status,
                Title = mapped.Title,
                Type = mapped.Type,
                Detail = mapped.Detail,
                Instance = context.Request.Path
            };

            if (!string.IsNullOrWhiteSpace(mapped.Code))
                pd.Extensions["code"] = mapped.Code;

            if (!string.IsNullOrWhiteSpace(mapped.ExtraDetails))
                pd.Extensions["details"] = mapped.ExtraDetails;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = pd,
                Exception = exception
            });

        }

        private static MappedProblem Map(Exception ex, IHostEnvironment env)
        {
            var isDev = env.IsDevelopment();

            // Sugestão: use URIs estáveis para "type" (documentáveis)
            // Pode ser um domínio interno, ou até paths do seu próprio swagger/docs.
            return ex switch
            {
                NotFoundException nf => new MappedProblem(
                    StatusCodes.Status404NotFound,
                    Title: "Recurso não encontrado",
                    Type: "https://errors.localhost/not-found",
                    Detail: nf.Message,
                    Code: "NOT_FOUND",
                    ExtraDetails: null
                ),

                BadRequestException br => new MappedProblem(
                    StatusCodes.Status400BadRequest,
                    Title: "Requisição inválida",
                    Type: "https://errors.localhost/bad-request",
                    Detail: br.Message,
                    Code: "BAD_REQUEST",
                    // Details é "seguro" para expor? Depende do seu padrão:
                    // - DEV: pode expor Details
                    // - PROD: expor só se você tiver certeza que não contém dados internos
                    ExtraDetails: isDev ? br.Details : null
                ),

                ValidationException ve => new MappedProblem(
                    StatusCodes.Status400BadRequest,
                    Title: "Erro de validação",
                    Type: "https://errors.localhost/validation",
                    Detail: "Um ou mais erros de validação ocorreram.",
                    Code: "VALIDATION_ERROR",
                    ExtraDetails: isDev ? ve.Message : null
                ),

                InternalServerException ise => new MappedProblem(
                    StatusCodes.Status500InternalServerError,
                    Title: "Erro interno",
                    Type: "https://errors.localhost/internal",
                    // Em PROD, nunca vaze detalhe interno em 500
                    Detail: isDev ? ise.Message : "Ocorreu um erro inesperado. Se persistir, contate o suporte.",
                    Code: "INTERNAL_ERROR",
                    ExtraDetails: isDev ? ise.Details : null
                ),

                // Timeouts/cancelamentos - opcional (ajuste conforme seu padrão)
                OperationCanceledException => new MappedProblem(
                    StatusCodes.Status499ClientClosedRequest, // não existe constante no StatusCodes
                    Title: "Requisição cancelada",
                    Type: "https://errors.localhost/request-cancelled",
                    Detail: "A requisição foi cancelada.",
                    Code: "REQUEST_CANCELLED",
                    ExtraDetails: null
                ),

                _ => new MappedProblem(
                    StatusCodes.Status500InternalServerError,
                    Title: "Erro interno",
                    Type: "https://errors.localhost/internal",
                    Detail: isDev ? ex.Message : "Ocorreu um erro inesperado. Se persistir, contate o suporte.",
                    Code: "UNHANDLED_EXCEPTION",
                    ExtraDetails: isDev ? ex.GetType().FullName : null
                )
            };
        }

        public record MappedProblem(int Status, string Title, string Type, string Detail, string? Code, string? ExtraDetails);
    }
}
