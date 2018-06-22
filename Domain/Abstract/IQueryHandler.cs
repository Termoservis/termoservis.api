namespace termoservis.api.Domain.Abstract
{
    public interface IQueryHandler<in TRequest, out TResponse>
        where TRequest : IQueryRequest
        where TResponse : IQueryResponse
    {
        TResponse Handle(TRequest request);
    }
}