// Owner: Reyes | Type: class (EF Core migration - creates the 6 tables of the ERD)
using System;
using GymApi.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymApi.Migrations
{
	[DbContext(typeof(AppDbContext))]
	[Migration("20261118100000_InitialCreate")]
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "UserAccounts",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
					Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserAccounts", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "MembershipPlans",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					PlanName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					DurationDays = table.Column<int>(type: "int", nullable: false),
					Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_MembershipPlans", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Members",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserAccountId = table.Column<int>(type: "int", nullable: true),
					FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
					Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					DateRegistered = table.Column<DateTime>(type: "date", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Members", x => x.Id);
					table.ForeignKey(
						name: "FK_Members_UserAccounts_UserAccountId",
						column: x => x.UserAccountId,
						principalTable: "UserAccounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ClassSchedules",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					InstructorId = table.Column<int>(type: "int", nullable: true),
					ClassDate = table.Column<DateTime>(type: "date", nullable: false),
					StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
					EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
					Capacity = table.Column<int>(type: "int", nullable: false),
					Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ClassSchedules", x => x.Id);
					table.ForeignKey(
						name: "FK_ClassSchedules_UserAccounts_InstructorId",
						column: x => x.InstructorId,
						principalTable: "UserAccounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Subscriptions",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					MemberId = table.Column<int>(type: "int", nullable: false),
					PlanId = table.Column<int>(type: "int", nullable: false),
					StartDate = table.Column<DateTime>(type: "date", nullable: false),
					ExpirationDate = table.Column<DateTime>(type: "date", nullable: false),
					Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Subscriptions", x => x.Id);
					table.ForeignKey(
						name: "FK_Subscriptions_Members_MemberId",
						column: x => x.MemberId,
						principalTable: "Members",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Subscriptions_MembershipPlans_PlanId",
						column: x => x.PlanId,
						principalTable: "MembershipPlans",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Reservations",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					MemberId = table.Column<int>(type: "int", nullable: false),
					ClassScheduleId = table.Column<int>(type: "int", nullable: false),
					ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Reservations", x => x.Id);
					table.ForeignKey(
						name: "FK_Reservations_ClassSchedules_ClassScheduleId",
						column: x => x.ClassScheduleId,
						principalTable: "ClassSchedules",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Reservations_Members_MemberId",
						column: x => x.MemberId,
						principalTable: "Members",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_UserAccounts_Username",
				table: "UserAccounts",
				column: "Username",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_MembershipPlans_PlanName",
				table: "MembershipPlans",
				column: "PlanName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Members_UserAccountId",
				table: "Members",
				column: "UserAccountId",
				unique: true,
				filter: "[UserAccountId] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_ClassSchedules_InstructorId",
				table: "ClassSchedules",
				column: "InstructorId");

			migrationBuilder.CreateIndex(
				name: "IX_Subscriptions_MemberId",
				table: "Subscriptions",
				column: "MemberId");

			migrationBuilder.CreateIndex(
				name: "IX_Subscriptions_PlanId",
				table: "Subscriptions",
				column: "PlanId");

			migrationBuilder.CreateIndex(
				name: "IX_Reservations_ClassScheduleId",
				table: "Reservations",
				column: "ClassScheduleId");

			migrationBuilder.CreateIndex(
				name: "IX_Reservations_MemberId_ClassScheduleId",
				table: "Reservations",
				columns: new[] { "MemberId", "ClassScheduleId" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Reservations");
			migrationBuilder.DropTable(name: "Subscriptions");
			migrationBuilder.DropTable(name: "ClassSchedules");
			migrationBuilder.DropTable(name: "Members");
			migrationBuilder.DropTable(name: "MembershipPlans");
			migrationBuilder.DropTable(name: "UserAccounts");
		}
	}
}
