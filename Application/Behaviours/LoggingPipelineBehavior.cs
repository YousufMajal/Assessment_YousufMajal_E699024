using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviours;

public class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> //generic constraints
    where TResponse : Result
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Request {@RequestName}, {@DateTime}", typeof(TRequest).Name, DateTime.Now);

        var result = await next();
        if (!result.IsSuccess)
        {
            _logger.LogError("Completed Request {@RequestName}, {@Error},{@DateTime}", typeof(TRequest).Name, result.Error, DateTime.Now);
        }

        _logger.LogInformation("Completed Request {@RequestName}, {@DateTime}", typeof(TRequest).Name, DateTime.Now);

        return result;
    }
}