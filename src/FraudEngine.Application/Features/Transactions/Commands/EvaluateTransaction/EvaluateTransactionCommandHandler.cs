using MediatR;
using Microsoft.Extensions.Logging;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Application.Features.Transactions.Commands.EvaluateTransaction;

public class EvaluateTransactionCommandHandler : IRequestHandler<EvaluateTransactionCommand, EvaluationResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFraudRuleEngine _ruleEngine;
    private readonly ILogger<EvaluateTransactionCommandHandler> _logger;

    public EvaluateTransactionCommandHandler(
        IUnitOfWork unitOfWork,
        IFraudRuleEngine ruleEngine,
        ILogger<EvaluateTransactionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _ruleEngine = ruleEngine;
        _logger = logger;
    }

    public async Task<EvaluationResultDto> Handle(EvaluateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            AccountNumber = request.AccountNumber,
            Amount = request.Amount,
            Currency = request.Currency,
            MerchantName = request.MerchantName,
            Category = request.Category,
            TransactionDate = request.TransactionDate,
            Location = request.Location,
            Country = request.Country,
            Channel = request.Channel
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var ruleResults = await _ruleEngine.EvaluateTransactionAsync(transaction, cancellationToken);
        var resultList = ruleResults.ToList();

        var alerts = resultList.Select(r => new FraudAlert
        {
            TransactionId = transaction.Id,
            RuleName = r.RuleName,
            Severity = r.Severity,
            Description = r.Description
        }).ToList();

        if (alerts.Any())
        {
            await _unitOfWork.FraudAlerts.AddRangeAsync(alerts, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Transaction {TransactionReference} flagged by {RuleCount} rules", transaction.TransactionReference, alerts.Count);
        }

        foreach (var alert in alerts)
        {
            alert.Transaction = transaction;
        }

        return new EvaluationResultDto
        {
            TransactionReference = transaction.TransactionReference,
            IsFlagged = alerts.Any(),
            AlertsGenerated = alerts.Count,
            Alerts = alerts.Select(a => a.ToDto()).ToList()
        };
    }
}
