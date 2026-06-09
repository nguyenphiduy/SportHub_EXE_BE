using FluentValidation;

namespace BidaPlatform.Application.UseCases.Subscriptions.AssignSubscription;

public class AssignSubscriptionValidator : AbstractValidator<AssignSubscriptionCommand>
{
    public AssignSubscriptionValidator()
    {
        RuleFor(x => x.VenueId)
            .NotEmpty().WithMessage("VenueId không hợp lệ");

        RuleFor(x => x.Request.EndDate)
            .GreaterThan(x => x.Request.StartDate)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu");
    }
}
