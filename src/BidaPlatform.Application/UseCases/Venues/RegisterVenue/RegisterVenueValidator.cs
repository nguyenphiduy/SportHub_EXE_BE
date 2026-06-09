using FluentValidation;

namespace BidaPlatform.Application.UseCases.Venues.RegisterVenue;

public class RegisterVenueValidator : AbstractValidator<RegisterVenueCommand>
{
    public RegisterVenueValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Tên quán không được để trống")
            .MaximumLength(255).WithMessage("Tên quán tối đa 255 ký tự");

        RuleFor(x => x.Request.OwnerName)
            .NotEmpty().WithMessage("Tên chủ quán không được để trống")
            .MaximumLength(255).WithMessage("Tên chủ quán tối đa 255 ký tự");

        RuleFor(x => x.Request.ManagerEmail)
            .NotEmpty().WithMessage("Email manager không được để trống")
            .EmailAddress().WithMessage("Email manager không hợp lệ");

        RuleFor(x => x.Request.ManagerFullName)
            .NotEmpty().WithMessage("Tên manager không được để trống")
            .MaximumLength(255).WithMessage("Tên manager tối đa 255 ký tự");

        RuleFor(x => x.Request.Phone)
            .MaximumLength(50).WithMessage("Số điện thoại tối đa 50 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Phone));

        RuleFor(x => x.Request.Address)
            .MaximumLength(500).WithMessage("Địa chỉ tối đa 500 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Address));
    }
}
