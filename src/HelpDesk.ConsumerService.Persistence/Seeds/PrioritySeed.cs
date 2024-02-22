using System;
using System.Collections.Generic;
using HelpDesk.ConsumerService.Domain.Entities;
using HelpDesk.ConsumerService.Domain.Enumerations;
using HelpDesk.ConsumerService.Persistence.Core.Primitives;

namespace HelpDesk.ConsumerService.Persistence.Seeds
{
    internal sealed class PrioritySeed : EntitySeedConfiguration<Priority, byte>
    {
        public override IEnumerable<object> Seed()
        {
            yield return new { Id = (byte)Priorities.Low, Name = "Baixa", Sla = 48, IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
            yield return new { Id = (byte)Priorities.Medium, Name = "Média", Sla = 24, IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
            yield return new { Id = (byte)Priorities.High, Name = "Alta", Sla = 8, IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
            yield return new { Id = (byte)Priorities.Criticial, Name = "Crítico", Sla = 4, IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
        }
    }
}
