using FluentValidation;
using BidaPlatform.Application.UseCases.Users.CreateManager;

namespace BidaPlatform.Application.Validators.Users;

public class CreateManagerValidator : AbstractValidator<CreateManagerCommand>
{
    public CreateManagerValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MinimumLength(2).WithMessage("Họ tên phải có ít nhất 2 ký tự.");
    }
}
