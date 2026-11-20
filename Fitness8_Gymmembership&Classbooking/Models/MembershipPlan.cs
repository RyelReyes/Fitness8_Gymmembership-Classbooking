// Owner: Reyes | Type: class (entity -> MEMBERSHIP_PLAN / table MembershipPlans)
using System.ComponentModel.DataAnnotations;

namespace GymApi.Models;

public class MembershipPlan
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string PlanName { get; set; } = string.Empty;

    public int DurationDays { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}
