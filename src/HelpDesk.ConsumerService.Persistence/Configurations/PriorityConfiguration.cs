using HelpDesk.ConsumerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.ConsumerService.Persistence.Configurations
{
    internal sealed class PriorityConfiguration : IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> builder)
        {
            builder.ToTable("priorities");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Sla);

            builder.Property(p => p.IsDeleted);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.LastUpdatedAt);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
