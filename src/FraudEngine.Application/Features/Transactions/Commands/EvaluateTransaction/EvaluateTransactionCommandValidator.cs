using FluentValidation;

namespace FraudEngine.Application.Features.Transactions.Commands.EvaluateTransaction;

public class EvaluateTransactionCommandValidator : AbstractValidator<EvaluateTransactionCommand>
{
    public EvaluateTransactionCommandValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("AccountNumber is required.");
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.MerchantName).NotEmpty().WithMessage("MerchantName is required.");
        RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("TransactionDate is required.");
    }
}
