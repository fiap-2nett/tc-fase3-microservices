using System;
using System.Collections.Generic;
using HelpDesk.Core.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using HelpDesk.ApiGateway.Persistence.Core.Abstractions;

namespace HelpDesk.ApiGateway.Persistence.Core.Primitives
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
