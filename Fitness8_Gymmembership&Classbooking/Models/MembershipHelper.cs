// Owner: Reyes | Type: static class (one place that decides "is this member's membership valid?")
namespace GymApi.Models;

public static class MembershipHelper
{
    /// <summary>
    /// Status: Inactive | None | Pending (starts later) | Active | Expired.
    /// Cancelled subscriptions are ignored. "Expired" is worked out from the dates, so no background job is needed.
    /// </summary>
    public static MembershipSummary Summarize(bool memberIsActive, IEnumerable<Subscription> subscriptions)
    {
        if (!memberIsActive) return new MembershipSummary("Inactive", null, null);

        var today = DateTime.Today;
        var open = subscriptions.Where(s => s.Status == SubscriptionStatus.Active).ToList();

        var current = open
            .Where(s => s.StartDate.Date <= today && s.ExpirationDate.Date >= today)
            .OrderByDescending(s => s.ExpirationDate)
            .FirstOrDefault();
        if (current is not null)
            return new MembershipSummary("Active", current.Plan?.PlanName, current.ExpirationDate);

        var latest = open.OrderByDescending(s => s.ExpirationDate).FirstOrDefault();
        if (latest is null) return new MembershipSummary("None", null, null);

        var status = latest.ExpirationDate.Date < today ? "Expired" : "Pending";
        return new MembershipSummary(status, latest.Plan?.PlanName, latest.ExpirationDate);
    }

    /// <summary>Stored status, but an "Active" subscription past its expiration date is shown as Expired.</summary>
    public static string EffectiveStatus(Subscription subscription)
    {
        if (subscription.Status == SubscriptionStatus.Active && subscription.ExpirationDate.Date < DateTime.Today)
            return SubscriptionStatus.Expired;

        return subscription.Status;
    }
}
