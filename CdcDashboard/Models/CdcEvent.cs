namespace CdcDashboard.Models;

public class CdcEvent<T>
{
    public string Uid { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public T? Before { get; set; }
    public T? After { get; set; }
    public Dictionary<string, (object? OldValue, object? NewValue)> Changes { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Name { get; set; } = string.Empty;
}

public class ApplicantEvent : CdcEvent<Applicant> { }
public class BookingEvent : CdcEvent<Booking> { }
public class PackageEvent : CdcEvent<Package> { }
