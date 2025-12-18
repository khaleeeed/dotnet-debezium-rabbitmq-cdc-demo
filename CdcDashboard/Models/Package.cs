using System.Text.Json.Serialization;

namespace CdcDashboard.Models;

public class Package
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int? Capacity { get; set; }
    public int? CapacityUsed { get; set; }
    public int? Status { get; set; }
   
    [JsonPropertyName("createdAt")]
    public long CreatedAtEpoch { get; set; }

    [JsonIgnore]
    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedAtEpoch).DateTime;
}

public class PackageMessage
{
    public ParentMessage<Package>? Payload { get; set; }
}
