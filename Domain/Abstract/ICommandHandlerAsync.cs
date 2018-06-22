using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public interface ICommandHandlerAsync<in TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}