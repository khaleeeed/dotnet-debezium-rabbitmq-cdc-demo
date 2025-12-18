using CdcDashboard.Models;
using CdcDashboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CdcDashboard.Pages;

public class ApplicantsModel : PageModel
{
    private readonly EventStore _eventStore;

    public ApplicantsModel(EventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public IEnumerable<ApplicantEvent> InitialEvents { get; private set; } = Enumerable.Empty<ApplicantEvent>();

    public void OnGet()
    {
        InitialEvents = _eventStore.GetApplicantEvents();
    }
}
