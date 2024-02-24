using HelpDesk.Core.Domain.Cryptography;
using HelpDesk.Core.Domain.Entities;
using HelpDesk.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.ApiGateway.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.IdRole).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Surname).HasMaxLength(150).IsRequired();
            builder.OwnsOne(p => p.Email, builder =>
            {
                builder.WithOwner();
                builder.Property(email => email.Value)
                    .HasColumnName(nameof(User.Email))
                    .HasMaxLength(Email.MaxLength)
                    .IsRequired();
            });
            builder.Property<string>("_passwordHash").HasField("_passwordHash").HasColumnName("PasswordHash").IsRequired();

            builder.Property(p => p.IsDeleted);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.LastUpdatedAt);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(p => p.IdRole).OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(p => !p.IsDeleted);         
        }
    }
}
