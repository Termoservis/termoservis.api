using System;
using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider serviceProvider;


        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        private TQueryHandler ResolveAsyncHandler<TQueryHandler, TRequest, TResponse>()
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TQueryHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve query handler. " + typeof(TQueryHandler).FullName);

            return (TQueryHandler)handler;
        }

        private TQueryHandler ResolveHandler<TQueryHandler, TRequest, TResponse>()
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TQueryHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve query handler. " + typeof(TQueryHandler).FullName);

            return (TQueryHandler)handler;
        }

        public TResponse Dispatch<TQueryHandler, TRequest, TResponse>(TRequest request)
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            if (request == null) 
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.ResolveHandler<TQueryHandler, TRequest, TResponse>()
                       .Handle(request);
        }

        public Task<TResponse> DispatchAsync<TQueryHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            if (request == null) 
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null) 
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            return this.ResolveAsyncHandler<TQueryHandler, TRequest, TResponse>()
                       .HandleAsync(request, cancellationToken);
        }
    }
}