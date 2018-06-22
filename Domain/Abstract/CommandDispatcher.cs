using System;
using System.Threading;
using System.Threading.Tasks;

namespace termoservis.api.Domain.Abstract
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider serviceProvider;


        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        private TCommandHandler ResolveAsyncHandler<TCommandHandler, TRequest, TResponse>()
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TCommandHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve command handler. " + typeof(TCommandHandler).FullName);

            return (TCommandHandler)handler;
        }

        private TCommandHandler ResolveHandler<TCommandHandler, TRequest, TResponse>()
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TCommandHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve command handler. " + typeof(TCommandHandler).FullName);

            return (TCommandHandler)handler;
        }
        
        public TResponse Dispatch<TCommandHandler, TRequest, TResponse>(TRequest request)
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.ResolveHandler<TCommandHandler, TRequest, TResponse>().Handle(request);
        }

        public Task<TResponse> DispatchAsync<TCommandHandler, TRequest, TResponse>(
            TRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            return this.ResolveAsyncHandler<TCommandHandler, TRequest, TResponse>().HandleAsync(request, cancellationToken);
        }
    }
}