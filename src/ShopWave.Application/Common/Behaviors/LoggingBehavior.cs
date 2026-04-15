using MediatR;
using Microsoft.Extensions.Logging;

namespace ShopWave.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("ShopWave Request: {Name} {@Request}", requestName, request);

        var response = await next(cancellationToken);

        logger.LogInformation("ShopWave Response: {Name} {@Response}", requestName, response);
        return response;
    }
}
