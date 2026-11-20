using System.ComponentModel.DataAnnotations;
namespace GymApi.Dtos;
public class LoginDto { [Required] public string Username { get; set; } = ""; [Required] public string Password { get; set; } = ""; }
public class RegisterDto { [Required, MinLength(3), MaxLength(50)] public string Username { get; set; } = ""; [Required, MinLength(6), MaxLength(100)] public string Password { get; set; } = ""; [Required, MaxLength(100)] public string FullName { get; set; } = ""; [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = ""; [MaxLength(20)] public string? Phone { get; set; } public string MembershipPlan { get; set; } = "Monthly"; }
public record AuthResponseDto(string Token, string Username, string FullName, string Role);
public record RegisterResponseDto(string Message, int MemberId, string MembershipPlan, DateTime MembershipExpiry);
