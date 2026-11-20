// Owner: Reyes | Type: class (entity -> MEMBER / table Members)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymApi.Models;

public class Member
{
    public int Id { get; set; }

    // Nullable on purpose (ERD): a walk-in member can be registered by the front desk without a login account.
    public int? UserAccountId { get; set; }
    public UserAccount? UserAccount { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [Column(TypeName = "date")]
    public DateTime DateRegistered { get; set; } = DateTime.Today;

    // Soft delete (added to the ERD): a deactivated member is kept for history.
    public bool IsActive { get; set; } = true;

    public string GetFullName() => FirstName + " " + LastName;
}
