using System;

namespace termoservis.api.Domain.Abstract
{
    public interface ICommandResponse
    {
        bool IsSuccess { get; set; }

        Exception Error { get; set; }
    }
}