using Microsoft.EntityFrameworkCore;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context) => _context = context;

    public async Task<Transaction?> GetByTransactionReferenceAsync(Guid transactionReference, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.FraudAlerts)
            .FirstOrDefaultAsync(t => t.TransactionReference == transactionReference, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions.Where(t => t.AccountNumber == accountNumber);

        if (from.HasValue)
            query = query.Where(t => t.TransactionDate >= from.Value);
        if (to.HasValue)
            query = query.Where(t => t.TransactionDate <= to.Value);

        return await query.OrderByDescending(t => t.TransactionDate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
    }
}
