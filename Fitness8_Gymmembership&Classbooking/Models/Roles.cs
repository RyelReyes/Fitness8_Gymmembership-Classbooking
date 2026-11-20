// Owner: Reyes | Type: static class (role name constants)
namespace GymApi.Models;

public static class Roles
{
    public const string Admin = "Admin";            // "Administrator" in the SAD
    public const string Helpdesk = "Helpdesk";      // front-desk personnel
    public const string Instructor = "Instructor";
    public const string Member = "Member";

    // Combinations used by [Authorize(Roles = ...)]
    public const string AdminOrHelpdesk = Admin + "," + Helpdesk;
    public const string AdminHelpdeskOrMember = Admin + "," + Helpdesk + "," + Member;

    public static readonly string[] StaffRoles = { Admin, Helpdesk, Instructor };
}
