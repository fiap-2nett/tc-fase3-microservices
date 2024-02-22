using System;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Primitives;
using HelpDesk.Core.Domain.Utility;

namespace HelpDesk.ConsumerService.Domain.Entities
{
    public sealed class Priority : Entity<byte>, IAuditableEntity, ISoftDeletableEntity
    {
        #region Properties

        public string Name { get; private set; }
        public int Sla { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private Priority()
        { }

        public Priority(byte idPriority, string name, int sla)
            : base(idPriority)
        {
            Ensure.GreaterThan(idPriority, 0, "The priority identifier must be greater than zero.", nameof(idPriority));
            Ensure.NotEmpty(name, "The priority name is required.", nameof(name));
            Ensure.GreaterThan(sla, 0, "The priority sla must be greater than zero.", nameof(sla));

            Name = name;
            Sla = sla;
        }

        #endregion
    }
}
