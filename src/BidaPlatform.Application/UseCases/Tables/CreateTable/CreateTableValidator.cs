using FluentValidation;

namespace BidaPlatform.Application.UseCases.Tables.CreateTable;

public class CreateTableValidator : AbstractValidator<CreateTableCommand>
{
    private static readonly string[] ValidTypes = ["Standard", "VIP", "Premium"];

    public CreateTableValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên bàn không được để trống")
            .MaximumLength(100).WithMessage("Tên bàn tối đa 100 ký tự");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Loại bàn không được để trống")
            .Must(t => ValidTypes.Contains(t)).WithMessage("Loại bàn phải là Standard, VIP hoặc Premium");

        RuleFor(x => x.PricePerHour)
            .GreaterThan(0).WithMessage("Giá mỗi giờ phải lớn hơn 0");
    }
}
