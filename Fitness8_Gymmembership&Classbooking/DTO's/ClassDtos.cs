namespace GymApi.Dtos;
public class GymClassInputDto { public string Name { get; set; } = ""; public string? Description { get; set; } public int InstructorId { get; set; } public DateTime StartTime { get; set; } public int DurationMinutes { get; set; } = 60; public int Capacity { get; set; } = 10; }
public record GymClassDto(int Id, string Name, string? Description, int? InstructorId, string InstructorName, DateTime StartTime, int DurationMinutes, int Capacity, int Reserved, int Available, string Status);
public record InstructorDto(int Id, string Name);
