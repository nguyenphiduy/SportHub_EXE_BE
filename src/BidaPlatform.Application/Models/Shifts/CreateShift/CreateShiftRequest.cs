namespace BidaPlatform.Application.Models.Shifts.CreateShift;

public class CreateShiftRequest
{
    public Guid? VenueId { get; set; }
    public Guid? StaffUserId { get; set; }
    public string Name { get; set; } = null!;
    public string ShiftDate { get; set; } = null!;   // Format: yyyy-MM-dd
    public string StartTime { get; set; } = null!;   // Format: HH:mm
    public string EndTime { get; set; } = null!;     // Format: HH:mm
}
