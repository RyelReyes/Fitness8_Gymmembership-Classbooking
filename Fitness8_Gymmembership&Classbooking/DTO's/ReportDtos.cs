namespace GymApi.Dtos;
public record OverdueMemberDto(string FullName, string? Email, string? MembershipPlan, DateTime? MembershipExpiry, int DaysOverdue);
public record ClassAttendanceDto(string ClassName, DateTime StartTime, int Capacity, int Reserved, int Confirmed, int Attended, int Cancelled);
