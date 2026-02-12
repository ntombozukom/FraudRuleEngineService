using FraudEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FraudEngine.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Action)
            .IsRequired();

        builder.Property(a => a.PropertyName)
            .HasMaxLength(100);

        builder.Property(a => a.OldValue)
            .HasMaxLength(4000);

        builder.Property(a => a.NewValue)
            .HasMaxLength(4000);

        builder.Property(a => a.ModifiedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.ModifiedAt)
            .IsRequired();

        builder.HasIndex(a => a.EntityType);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.ModifiedBy);
        builder.HasIndex(a => a.ModifiedAt);
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
    }
}
