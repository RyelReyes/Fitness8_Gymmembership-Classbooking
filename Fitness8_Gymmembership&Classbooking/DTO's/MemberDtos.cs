namespace GymApi.Dtos;
public class MemberCreateDto { public string Username { get; set; } = ""; public string Password { get; set; } = ""; public string FullName { get; set; } = ""; public string? Email { get; set; } public string? Phone { get; set; } public string? MembershipPlan { get; set; } }
public class MemberUpdateDto { public string FullName { get; set; } = ""; public string? Email { get; set; } public string? Phone { get; set; } public bool IsActive { get; set; } = true; }
public record MemberDto(int Id, string FullName, string Username, string? Email, string? Phone, string? MembershipPlan, DateTime? MembershipExpiry, string MembershipStatus, bool IsActive);
public class MemberRenewDto { public string PlanName { get; set; } = "Monthly"; }
