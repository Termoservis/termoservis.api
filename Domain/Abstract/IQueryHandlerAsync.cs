using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public interface IQueryHandlerAsync<in TRequest, TResponse>
        where TRequest : IQueryRequest
        where TResponse : IQueryResponse
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}