using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public interface ICommandDispatcher 
    {
        Task<TResponse> DispatchAsync<TCommandHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;

        TResponse Dispatch<TCommandHandler, TRequest, TResponse>(TRequest request)
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;
    }
}