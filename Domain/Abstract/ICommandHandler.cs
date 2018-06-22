namespace termoservis.api.Domain.Abstract
{
    public interface ICommandHandler<in TRequest, out TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        TResponse Handle(TRequest request);
    }
}