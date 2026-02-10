using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Infrastructure.Persistence.Configurations;

public class FraudAlertConfiguration : IEntityTypeConfiguration<FraudAlert>
{
    public void Configure(EntityTypeBuilder<FraudAlert> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AlertReference).IsRequired();
        builder.Property(a => a.RuleName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(500);
        builder.Property(a => a.Severity).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.ReviewedBy).HasMaxLength(200);

        builder.HasIndex(a => a.AlertReference).IsUnique();
        builder.HasIndex(a => a.TransactionId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => a.CreatedAt);

        builder.HasOne(a => a.Transaction)
            .WithMany(t => t.FraudAlerts)
            .HasForeignKey(a => a.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
