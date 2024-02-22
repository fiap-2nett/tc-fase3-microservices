using System;
using HelpDesk.Core.Domain.Primitives;

namespace HelpDesk.Core.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public Error Error { get; }

        public NotFoundException(Error error) : base(error.Message)
            => Error = error;
    }
}
