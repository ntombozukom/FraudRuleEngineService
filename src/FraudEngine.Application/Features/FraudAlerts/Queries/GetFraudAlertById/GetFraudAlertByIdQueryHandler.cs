using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertById;

public class GetFraudAlertByReferenceQueryHandler : IRequestHandler<GetFraudAlertByReferenceQuery, FraudAlertDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFraudAlertByReferenceQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FraudAlertDto?> Handle(GetFraudAlertByReferenceQuery request, CancellationToken cancellationToken)
    {
        var alert = await _unitOfWork.FraudAlerts.GetByAlertReferenceAsync(request.AlertReference, cancellationToken);
        return alert?.ToDto();
    }
}
