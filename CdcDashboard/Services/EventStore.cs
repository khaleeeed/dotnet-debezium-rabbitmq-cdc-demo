using System.Collections.Concurrent;
using CdcDashboard.Models;

namespace CdcDashboard.Services;

public class EventStore
{
    private readonly ConcurrentQueue<ApplicantEvent> _applicantEvents = new();
    private readonly ConcurrentQueue<BookingEvent> _bookingEvents = new();
    private readonly ConcurrentQueue<PackageEvent> _packageEvents = new();
    
    private const int MaxEvents = 100;

    public void AddApplicantEvent(ApplicantEvent evt)
    {
        _applicantEvents.Enqueue(evt);
        while (_applicantEvents.Count > MaxEvents)
            _applicantEvents.TryDequeue(out _);
    }

    public void AddBookingEvent(BookingEvent evt)
    {
        _bookingEvents.Enqueue(evt);
        while (_bookingEvents.Count > MaxEvents)
            _bookingEvents.TryDequeue(out _);
    }

    public void AddPackageEvent(PackageEvent evt)
    {
        _packageEvents.Enqueue(evt);
        while (_packageEvents.Count > MaxEvents)
            _packageEvents.TryDequeue(out _);
    }

    public IEnumerable<ApplicantEvent> GetApplicantEvents() => _applicantEvents.Reverse();
    public IEnumerable<BookingEvent> GetBookingEvents() => _bookingEvents.Reverse();
    public IEnumerable<PackageEvent> GetPackageEvents() => _packageEvents.Reverse();

    public (int Creates, int Updates, int Deletes) GetApplicantCounts()
    {
        var events = _applicantEvents.ToList();
        return (
            events.Count(e => e.Type == "Create"),
            events.Count(e => e.Type == "Update"),
            events.Count(e => e.Type == "Delete")
        );
    }

    public (int Today, decimal Revenue, int Pending, int Confirmed) GetBookingKpis()
    {
        var events = _bookingEvents.ToList();
        var today = events.Count(e => e.Timestamp.Date == DateTime.UtcNow.Date);
        var revenue = events.Where(e => e.After?.Amount != null).Sum(e => e.After!.Amount!.Value);
        var pending = events.Count(e => e.After?.BookingStatus == 1);
        var confirmed = events.Count(e => e.After?.BookingStatus == 2);
        return (today, revenue, pending, confirmed);
    }
}
