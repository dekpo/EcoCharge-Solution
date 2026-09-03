using EcoCharge.Application.ChargeSessions.Dtos;
using EcoCharge.Application.Common.Interfaces;
using MediatR;

namespace EcoCharge.Application.ChargeSessions.Queries.GetActiveChargeSessions;

public sealed class GetActiveChargeSessionsQueryHandler(IChargeSessionRepository repository)
    : IRequestHandler<GetActiveChargeSessionsQuery, IReadOnlyList<ChargeSessionDto>>
{
    public async Task<IReadOnlyList<ChargeSessionDto>> Handle(GetActiveChargeSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await repository.GetActiveSessionsAsync(cancellationToken);
        return sessions.Select(ChargeSessionDto.FromEntity).ToList();
    }
}
