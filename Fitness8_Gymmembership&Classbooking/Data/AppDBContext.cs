// Owner: Reyes | Type: class (EF Core DbContext - the 6 tables of the ERD)
using GymApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GymApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique keys from the ERD (UK): username and plan name.
        modelBuilder.Entity<UserAccount>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<MembershipPlan>().HasIndex(p => p.PlanName).IsUnique();

        // Every relationship is Restrict: nothing is hard-deleted (status / soft delete instead),
        // and it avoids SQL Server "multiple cascade paths" errors.
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasIndex(m => m.UserAccountId).IsUnique();
            entity.HasOne(m => m.UserAccount).WithMany().HasForeignKey(m => m.UserAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasOne(s => s.Member).WithMany().HasForeignKey(s => s.MemberId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(s => s.Plan).WithMany().HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.HasOne(c => c.Instructor).WithMany().HasForeignKey(c => c.InstructorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasIndex(r => new { r.MemberId, r.ClassScheduleId });
            entity.HasOne(r => r.Member).WithMany().HasForeignKey(r => r.MemberId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.ClassSchedule).WithMany().HasForeignKey(r => r.ClassScheduleId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
