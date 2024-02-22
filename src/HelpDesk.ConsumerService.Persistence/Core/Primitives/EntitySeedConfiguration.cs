using System;
using System.Collections.Generic;
using HelpDesk.Core.Domain.Primitives;
using HelpDesk.ConsumerService.Persistence.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.ConsumerService.Persistence.Core.Primitives
{
    internal abstract class EntitySeedConfiguration<TEntity, TKey> : IEntitySeedConfiguration
        where TEntity : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        #region IEntitySeedConfiguration Members

        public abstract IEnumerable<object> Seed();

        public void Configure(ModelBuilder modelBuilder)
            => modelBuilder.Entity<TEntity>().HasData(Seed());

        #endregion
    }
}
