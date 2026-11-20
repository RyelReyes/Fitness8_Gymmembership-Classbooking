// Owner: Reyes | Type: static class (reservation status workflow rules)
namespace GymApi.Models;

/// <summary>
/// Reservation lifecycle:  Reserved -> Confirmed -> Attended,
/// and Reserved/Confirmed -> Cancelled. Attended and Cancelled are final.
/// </summary>
public static class ReservationStatus
{
    public const string Reserved = "Reserved";
    public const string Confirmed = "Confirmed";
    public const string Attended = "Attended";
    public const string Cancelled = "Cancelled";

    private static readonly Dictionary<string, string[]> Transitions = new()
    {
        [Reserved] = new[] { Confirmed, Cancelled },
        [Confirmed] = new[] { Attended, Cancelled },
        [Attended] = Array.Empty<string>(),
        [Cancelled] = Array.Empty<string>()
    };

    public static bool IsValid(string status) => Transitions.ContainsKey(status);

    public static string[] NextStatuses(string from) =>
        Transitions.TryGetValue(from, out var next) ? next : Array.Empty<string>();

    public static bool CanMove(string from, string to) => NextStatuses(from).Contains(to);
}
