using System;
using HelpDesk.Core.Domain.Primitives;

namespace HelpDesk.Core.Domain.Exceptions
{
    public class InvalidPermissionException : Exception
    {
        public Error Error { get; }

        public InvalidPermissionException(Error error) : base(error.Message)
            => Error = error;
    }
}
