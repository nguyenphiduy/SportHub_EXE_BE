using MediatR;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Sessions.UpdatePayment;

public class UpdateSessionPaymentHandler : IRequestHandler<UpdateSessionPaymentCommand>
{
    private readonly IBilliardSessionRepository _sessionRepo;

    public UpdateSessionPaymentHandler(IBilliardSessionRepository sessionRepo)
        => _sessionRepo = sessionRepo;

    public async Task Handle(UpdateSessionPaymentCommand request, CancellationToken ct)
    {
        var session = await _sessionRepo.GetByIdAsync(request.SessionId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiên chơi {request.SessionId}");

        if (request.NewPaymentMethod.HasValue)
            session.PaymentMethod = request.NewPaymentMethod.Value;

        if (request.MarkAsPaid)
            session.PaymentStatus = BilliardPaymentStatus.Paid;

        _sessionRepo.Update(session);
        await _sessionRepo.SaveChangesAsync(ct);
    }
}
