
using System.Text.Json.Serialization;

namespace DataStreaming.Consumer;

public class Booking
{
    public int Id { get; set; }
    public int? PackageId { get; set; }
    public int? ApplicantId { get; set; }
    public int? BookingStatus { get; set; }
    public decimal? Amount { get; set; }

    [JsonPropertyName("BookingDate")]
    public long? BookingDateEpoch { get; set; }

    [JsonIgnore]
    public DateTime? BookingDate => BookingDateEpoch.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(BookingDateEpoch.Value).DateTime : null;

    [JsonPropertyName("createdAt")]
    public long CreatedAtEpoch { get; set; }

    [JsonIgnore]
    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedAtEpoch).DateTime;
}

public class BookingMessage
{
    public ParentMessage<Booking>? Payload { get; set; }
}
