// Owner: Reyes | Type: static class (seed test accounts + sample data)
using GymApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GymApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher<UserAccount> hasher)
    {
        if (await db.UserAccounts.AnyAsync()) return;   // already seeded

        // ---- staff accounts ----
        var admin = CreateAccount(hasher, "admin", "Admin123!", "Gym Administrator", Roles.Admin);
        var desk = CreateAccount(hasher, "helpdesk", "Help123!", "Front Desk Staff", Roles.Helpdesk);
        var mia = CreateAccount(hasher, "coach.mia", "Coach123!", "Coach Mia Santos", Roles.Instructor);
        var leo = CreateAccount(hasher, "coach.leo", "Coach123!", "Coach Leo Reyes", Roles.Instructor);
        db.UserAccounts.AddRange(admin, desk, mia, leo);

        // ---- membership plans ----
        var monthly = new MembershipPlan { PlanName = "Monthly", DurationDays = 30, Description = "1 month access" };
        var quarterly = new MembershipPlan { PlanName = "Quarterly", DurationDays = 90, Description = "3 months access" };
        var annual = new MembershipPlan { PlanName = "Annual", DurationDays = 365, Description = "12 months access" };
        db.MembershipPlans.AddRange(monthly, quarterly, annual);

        // ---- members ----
        var juan = CreateMember(hasher, "juan", "Juan", "Dela Cruz", "0917-100-0001", "juan@example.com");
        var maria = CreateMember(hasher, "maria", "Maria", "Santos", "0917-100-0002", "maria@example.com");
        var pedro = CreateMember(hasher, "pedro", "Pedro", "Reyes", "0917-100-0003", "pedro@example.com");
        var ana = CreateMember(hasher, "ana", "Ana", "Lopez", "0917-100-0004", "ana@example.com");
        var walkIn = new Member { FirstName = "Rico", LastName = "Tan", ContactNumber = "0917-100-0005" };   // walk-in, no login
        db.Members.AddRange(juan, maria, pedro, ana, walkIn);
        await db.SaveChangesAsync();

        // ---- subscriptions (maria's is already past its expiration date -> Expired) ----
        var today = DateTime.Today;
        db.Subscriptions.AddRange(
            NewSubscription(juan, monthly, today.AddDays(-10)),
            NewSubscription(maria, monthly, today.AddDays(-40)),
            NewSubscription(pedro, annual, today.AddDays(-30)),
            NewSubscription(ana, quarterly, today.AddDays(-5)));

        // ---- classes (HIIT Blast has capacity 2, Boxing Basics overlaps it) ----
        var yoga = NewClass("Morning Yoga", mia, today.AddDays(1), 7, 0, 8, 0, 10);
        var hiit = NewClass("HIIT Blast", mia, today.AddDays(1), 18, 0, 18, 45, 2);
        var boxing = NewClass("Boxing Basics", leo, today.AddDays(1), 18, 30, 19, 15, 12);
        var zumba = NewClass("Zumba Party", leo, today.AddDays(2), 9, 0, 10, 0, 15);
        db.ClassSchedules.AddRange(yoga, hiit, boxing, zumba);
        await db.SaveChangesAsync();

        // HIIT Blast is already full so the "class is full" rule can be demoed.
        db.Reservations.AddRange(
            new Reservation { MemberId = pedro.Id, ClassScheduleId = hiit.Id, Status = ReservationStatus.Confirmed },
            new Reservation { MemberId = ana.Id, ClassScheduleId = hiit.Id, Status = ReservationStatus.Reserved });
        await db.SaveChangesAsync();
    }

    private static UserAccount CreateAccount(IPasswordHasher<UserAccount> hasher, string username, string password, string fullName, string role)
    {
        var account = new UserAccount { Username = username, FullName = fullName, Role = role };
        account.PasswordHash = hasher.HashPassword(account, password);
        return account;
    }

    private static Member CreateMember(IPasswordHasher<UserAccount> hasher, string username, string first, string last, string contact, string email)
    {
        return new Member
        {
            UserAccount = CreateAccount(hasher, username, "Member123!", first + " " + last, Roles.Member),
            FirstName = first,
            LastName = last,
            ContactNumber = contact,
            Email = email
        };
    }

    private static Subscription NewSubscription(Member member, MembershipPlan plan, DateTime start)
    {
        return new Subscription
        {
            MemberId = member.Id,
            PlanId = plan.Id,
            StartDate = start,
            ExpirationDate = start.AddDays(plan.DurationDays),
            Status = SubscriptionStatus.Active
        };
    }

    private static ClassSchedule NewClass(string name, UserAccount instructor, DateTime date, int startHour, int startMinute, int endHour, int endMinute, int capacity)
    {
        return new ClassSchedule
        {
            ClassName = name,
            InstructorId = instructor.Id,
            ClassDate = date,
            StartTime = new TimeSpan(startHour, startMinute, 0),
            EndTime = new TimeSpan(endHour, endMinute, 0),
            Capacity = capacity,
            Status = ClassStatus.Open
        };
    }
}
