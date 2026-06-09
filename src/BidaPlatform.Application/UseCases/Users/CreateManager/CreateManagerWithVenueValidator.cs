using FluentValidation;
using BidaPlatform.Application.UseCases.Users.CreateManager;

namespace BidaPlatform.Application.Validators.Users;

public class CreateManagerWithVenueValidator : AbstractValidator<CreateManagerWithVenueCommand>
{
    public CreateManagerWithVenueValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email khong duoc de trong.")
            .EmailAddress().WithMessage("Email khong hop le.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ho ten khong duoc de trong.")
            .MinimumLength(2).WithMessage("Ho ten phai co it nhat 2 ky tu.");

        RuleFor(x => x.VenueName)
            .NotEmpty().WithMessage("Ten quan khong duoc de trong.");
    }
}
