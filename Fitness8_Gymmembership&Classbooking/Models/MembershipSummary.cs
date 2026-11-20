// Owner: Reyes | Type: record (result of MembershipHelper.Summarize)
namespace GymApi.Models;

public record MembershipSummary(string Status, string? PlanName, DateTime? ExpiresOn);
