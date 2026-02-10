using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TransactionReference).IsRequired();
        builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Currency).HasMaxLength(3);
        builder.Property(t => t.MerchantName).HasMaxLength(200);
        builder.Property(t => t.Category).HasMaxLength(100);
        builder.Property(t => t.Location).HasMaxLength(200);
        builder.Property(t => t.Country).HasMaxLength(5);
        builder.Property(t => t.Channel).HasConversion<string>().HasMaxLength(13);

        builder.Property(t => t.AccountNumber).HasMaxLength(13).IsRequired();

        builder.HasIndex(t => t.TransactionReference).IsUnique();
        builder.HasIndex(t => t.AccountNumber);
        builder.HasIndex(t => t.TransactionDate);
    }
}
