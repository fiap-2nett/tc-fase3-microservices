using System;

namespace HelpDesk.Core.Domain.Abstractions
{
    public interface IAuditableEntity
    {
        #region IAuditableEntity Members

        DateTime CreatedAt { get; }
        DateTime? LastUpdatedAt { get; }

        #endregion
    }
}
