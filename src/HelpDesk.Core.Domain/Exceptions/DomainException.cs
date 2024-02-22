using System;
using HelpDesk.Core.Domain.Primitives;

namespace HelpDesk.Core.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public Error Error { get; }

        public DomainException(Error error) : base(error.Message)
            => Error = error;
    }
}
