// Owner: Reyes | Type: class (entity -> USER_ACCOUNT / table UserAccounts)
using System.ComponentModel.DataAnnotations;

namespace GymApi.Models;

public class UserAccount
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Role { get; set; } = Roles.Member;

    [Required, MaxLength(10)]
    public string Status { get; set; } = AccountStatus.Active;
}
