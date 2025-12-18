using CdcDashboard.Models;
using CdcDashboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CdcDashboard.Pages;

public class BookingsModel : PageModel
{
    private readonly EventStore _eventStore;

    public BookingsModel(EventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public IEnumerable<BookingEvent> InitialEvents { get; private set; } = Enumerable.Empty<BookingEvent>();
    public Dictionary<int, string> ApplicantNames { get; private set; } = new();
    public Dictionary<int, string> PackageNames { get; private set; } = new();

    public void OnGet()
    {
        InitialEvents = _eventStore.GetBookingEvents();

        // Build Applicant Lookup
        var applicants = new Dictionary<int, string>();
        foreach (var evt in _eventStore.GetApplicantEvents().Reverse()) // Chronological order
        {
            if (evt.After != null)
            {
                applicants[evt.After.Id] = evt.After.FullName;
            }
        }
        ApplicantNames = applicants;

        // Build Package Lookup
        var packages = new Dictionary<int, string>();
        foreach (var evt in _eventStore.GetPackageEvents().Reverse()) // Chronological order
        {
             if (evt.After != null && evt.After.Id.HasValue)
            {
                packages[evt.After.Id.Value] = evt.After.Name ?? "Unknown Package";
            }
        }
        PackageNames = packages;
    }
}
