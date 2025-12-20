using System.Text.Json.Serialization;

namespace DataStreaming.Consumer;

public class Applicant
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    [JsonPropertyName("BirthDate")]
    public long? BirthDateEpoch { get; set; }

    [JsonIgnore]
    public DateTime? BirthDate => BirthDateEpoch.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(BirthDateEpoch.Value).DateTime : null;


    [JsonPropertyName("created_at")]
    public long CreatedAtEpoch { get; set; }

    [JsonIgnore]
    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedAtEpoch).DateTime;

}

public class ApplicantMessage
{
    public ParentMessage<Applicant>? Payload { get; set; }
}
