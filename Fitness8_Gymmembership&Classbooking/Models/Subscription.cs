// Owner: Reyes | Type: class (entity -> SUBSCRIPTION / table Subscriptions)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymApi.Models;

public class Subscription
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public int PlanId { get; set; }
    public MembershipPlan? Plan { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime ExpirationDate { get; set; }

    [Required, MaxLength(10)]
    public string Status { get; set; } = SubscriptionStatus.Active;
}
