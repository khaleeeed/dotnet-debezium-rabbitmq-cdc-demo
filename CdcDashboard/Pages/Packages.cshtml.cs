using CdcDashboard.Models;
using CdcDashboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CdcDashboard.Pages;

public class PackagesModel : PageModel
{
    private readonly EventStore _eventStore;

    public PackagesModel(EventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public IEnumerable<PackageEvent> InitialEvents { get; private set; } = Enumerable.Empty<PackageEvent>();

    public void OnGet()
    {
        InitialEvents = _eventStore.GetPackageEvents();
    }
}
