using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQueryHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;

        TResponse Dispatch<TQueryHandler, TRequest, TResponse>(TRequest request)
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;
    }
}