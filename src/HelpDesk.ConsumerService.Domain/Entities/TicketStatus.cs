using System;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Primitives;
using HelpDesk.Core.Domain.Utility;

namespace HelpDesk.ConsumerService.Domain.Entities
{
    public sealed class TicketStatus : Entity<byte>, IAuditableEntity, ISoftDeletableEntity
    {
        #region Properties

        public string Name { get; private set; }
        public string Description { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private TicketStatus()
        { }

        public TicketStatus(byte idTicketStatus, string name, string description = null)
            : base(idTicketStatus)
        {
            Ensure.GreaterThan(idTicketStatus, 0, "The ticket status identifier must be greater than zero.", nameof(idTicketStatus));
            Ensure.NotEmpty(name, "The ticket status name is required.", nameof(name));

            Name = name;
            Description = description;
        }

        #endregion
    }
}
