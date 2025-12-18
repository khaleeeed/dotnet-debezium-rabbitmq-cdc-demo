using System.Collections.Concurrent;
using CdcDashboard.Models;

namespace CdcDashboard.Services;

public class EventStore
{
    public readonly ConcurrentQueue<ApplicantEvent> ApplicantEvents = new();
    public readonly ConcurrentQueue<BookingEvent> BookingEvents = new();
    public readonly ConcurrentQueue<PackageEvent> PackageEvents = new();
    
    private const int MaxEvents = 100;

    public void AddApplicantEvent(ApplicantEvent evt)
    {
        ApplicantEvents.Enqueue(evt);
        while (ApplicantEvents.Count > MaxEvents)
            ApplicantEvents.TryDequeue(out _);
    }

    public void AddBookingEvent(BookingEvent evt)
    {
        BookingEvents.Enqueue(evt);
        while (BookingEvents.Count > MaxEvents)
            BookingEvents.TryDequeue(out _);
    }

    public void AddPackageEvent(PackageEvent evt)
    {
        PackageEvents.Enqueue(evt);
        while (PackageEvents.Count > MaxEvents)
            PackageEvents.TryDequeue(out _);
    }

    public IEnumerable<ApplicantEvent> GetApplicantEvents() => ApplicantEvents.Reverse();
    public IEnumerable<BookingEvent> GetBookingEvents() => BookingEvents.Reverse();
    public IEnumerable<PackageEvent> GetPackageEvents() => PackageEvents.Reverse();

    public (int Creates, int Updates, int Deletes) GetApplicantCounts()
    {
        var events = ApplicantEvents.ToList();
        return (
            events.Count(e => e.Type == "Create"),
            events.Count(e => e.Type == "Update"),
            events.Count(e => e.Type == "Delete")
        );
    }

    public (int Today, decimal Revenue, int Pending, int Confirmed) GetBookingKpis()
    {
        var events = BookingEvents.ToList();
        var today = events.Count(e => e.Timestamp.Date == DateTime.UtcNow.Date);
        var revenue = events.Where(e => e.After?.Amount != null).Sum(e => e.After!.Amount!.Value);
        var pending = events.Count(e => e.After?.BookingStatus == 1);
        var confirmed = events.Count(e => e.After?.BookingStatus == 2);
        return (today, revenue, pending, confirmed);
    }
}
