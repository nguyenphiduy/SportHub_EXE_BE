using FluentValidation;

namespace BidaPlatform.Application.UseCases.Venues.UpdateVenue;

public class UpdateVenueValidator : AbstractValidator<UpdateVenueCommand>
{
    public UpdateVenueValidator()
    {
        RuleFor(x => x.VenueId)
            .NotEmpty().WithMessage("VenueId không hợp lệ");

        RuleFor(x => x.Request.Name)
            .MaximumLength(255).WithMessage("Tên quán tối đa 255 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Name));

        RuleFor(x => x.Request.OwnerName)
            .MaximumLength(255).WithMessage("Tên chủ quán tối đa 255 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.OwnerName));

        RuleFor(x => x.Request.Phone)
            .MaximumLength(50).WithMessage("Số điện thoại tối đa 50 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Phone));

        RuleFor(x => x.Request.Address)
            .MaximumLength(500).WithMessage("Địa chỉ tối đa 500 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Address));
    }
}
