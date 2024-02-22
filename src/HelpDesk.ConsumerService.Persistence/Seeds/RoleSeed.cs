using System;
using System.Collections.Generic;
using HelpDesk.ConsumerService.Domain.Entities;
using HelpDesk.ConsumerService.Domain.Enumerations;
using HelpDesk.ConsumerService.Persistence.Core.Primitives;
using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.ConsumerService.Persistence.Seeds
{
    internal sealed class RoleSeed : EntitySeedConfiguration<Role, byte>
    {
        public override IEnumerable<object> Seed()
        {
            yield return new { Id = (byte)UserRoles.Administrator, Name = "Administrador", IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
            yield return new { Id = (byte)UserRoles.General, Name = "Geral", IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
            yield return new { Id = (byte)UserRoles.Analyst, Name = "Analista", IsDeleted = false, CreatedAt = DateTime.MinValue.Date };
        }
    }
}
