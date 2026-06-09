using FluentValidation;

namespace BidaPlatform.Application.UseCases.Shifts.CreateShift;

public class CreateShiftValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftValidator()
    {
        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request!.Name)
                .NotEmpty().WithMessage("Tên ca không được để trống")
                .MaximumLength(150).WithMessage("Tên ca tối đa 150 ký tự");

            RuleFor(x => x.Request!.ShiftDate)
                .NotEmpty().WithMessage("Ngày không được để trống")
                .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Ngày phải theo định dạng yyyy-MM-dd");

            RuleFor(x => x.Request!.StartTime)
                .NotEmpty().WithMessage("Giờ bắt đầu không được để trống")
                .Matches(@"^\d{2}:\d{2}$").WithMessage("Giờ bắt đầu phải theo định dạng HH:mm");

            RuleFor(x => x.Request!.EndTime)
                .NotEmpty().WithMessage("Giờ kết thúc không được để trống")
                .Matches(@"^\d{2}:\d{2}$").WithMessage("Giờ kết thúc phải theo định dạng HH:mm")
                .GreaterThan(x => x.Request!.StartTime).WithMessage("Giờ kết thúc phải lớn hơn giờ bắt đầu");
        });
    }
}
