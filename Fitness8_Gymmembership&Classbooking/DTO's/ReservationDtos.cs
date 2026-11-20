namespace GymApi.Dtos;
public class ReservationCreateDto { public int ClassId { get; set; } public int ClassScheduleId { get; set; } public int GymClassId { get; set; } }
public class ReservationStatusDto { public string Status { get; set; } = ""; }
public record ReservationDto(int Id, string MemberName, string ClassName, DateTime ClassStartTime, string Status, DateTime CreatedAt, string[] AllowedStatuses);
