using FraudEngine.Domain.Entities;

namespace FraudEngine.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByTransactionReferenceAsync(Guid transactionReference, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
