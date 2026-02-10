using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Commands.ReviewFraudAlert;

public class ReviewFraudAlertCommandHandler : IRequestHandler<ReviewFraudAlertCommand, FraudAlertDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewFraudAlertCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FraudAlertDto?> Handle(ReviewFraudAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _unitOfWork.FraudAlerts.GetByAlertReferenceAsync(request.AlertReference, cancellationToken);
        if (alert is null) return null;

        alert.Status = request.Status;
        alert.ReviewedBy = request.ReviewedBy;
        alert.ReviewedAt = DateTime.UtcNow;

        await _unitOfWork.FraudAlerts.UpdateAsync(alert, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return alert.ToDto();
    }
}
