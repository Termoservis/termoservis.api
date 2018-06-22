using System;

namespace termoservis.api.Domain.Abstract
{
    public class CommandResponse : ICommandResponse
    {
        public bool IsSuccess { get; set; }

        public Exception Error { get; set; }
    }
}