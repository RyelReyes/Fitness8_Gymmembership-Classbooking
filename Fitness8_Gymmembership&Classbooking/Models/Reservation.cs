// Owner: Reyes | Type: class (entity -> RESERVATION / table Reservations)
using System.ComponentModel.DataAnnotations;

namespace GymApi.Models;

public class Reservation
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public int ClassScheduleId { get; set; }
    public ClassSchedule? ClassSchedule { get; set; }

    public DateTime ReservationDate { get; set; } = DateTime.Now;

    [Required, MaxLength(10)]
    public string Status { get; set; } = ReservationStatus.Reserved;
}
