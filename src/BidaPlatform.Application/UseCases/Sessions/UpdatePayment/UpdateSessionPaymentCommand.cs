using MediatR;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.UseCases.Sessions.UpdatePayment;

/// <summary>
/// Update payment method and/or mark a completed session as Paid.
/// </summary>
public record UpdateSessionPaymentCommand(
    Guid SessionId,
    BilliardPaymentMethod? NewPaymentMethod = null,
    bool MarkAsPaid = false) : IRequest;
