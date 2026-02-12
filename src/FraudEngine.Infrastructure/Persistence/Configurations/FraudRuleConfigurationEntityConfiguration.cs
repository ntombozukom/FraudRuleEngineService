using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Infrastructure.Persistence.Configurations;

public class FraudRuleConfigurationEntityConfiguration : IEntityTypeConfiguration<FraudRuleConfiguration>
{
    public void Configure(EntityTypeBuilder<FraudRuleConfiguration> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RuleName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Parameters).HasMaxLength(1000);
        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.CreatedBy).HasMaxLength(256).IsRequired();
        builder.Property(r => r.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(r => r.RuleName).IsUnique();
    }
}
