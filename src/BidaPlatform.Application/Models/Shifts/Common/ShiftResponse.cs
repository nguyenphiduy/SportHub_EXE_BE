namespace BidaPlatform.Application.Models.Shifts.Common;

public class ShiftResponse
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid? StaffUserId { get; set; }
    public string? StaffUserName { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ShiftDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public bool IsActive { get; set; }
}
