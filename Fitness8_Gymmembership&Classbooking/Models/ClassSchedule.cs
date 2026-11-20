// Owner: Reyes | Type: class (entity -> CLASS_SCHEDULE / table ClassSchedules)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymApi.Models;

public class ClassSchedule
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;

    // Nullable (ERD): a class may not have an instructor assigned yet.
    public int? InstructorId { get; set; }
    public UserAccount? Instructor { get; set; }

    [Column(TypeName = "date")]
    public DateTime ClassDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int Capacity { get; set; }

    [Required, MaxLength(10)]
    public string Status { get; set; } = ClassStatus.Open;

    public DateTime GetStart() => ClassDate.Date + StartTime;

    public DateTime GetEnd() => ClassDate.Date + EndTime;
}
