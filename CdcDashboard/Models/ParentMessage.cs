using System.Reflection;

namespace CdcDashboard.Models;

public enum ChangeOperation
{
    Unknown = 0,
    Create,
    Update,
    Delete
}

public class ParentMessage<T>
{
    public T? Before { get; set; }
    public T? After { get; set; }
    public string? Op { get; set; }

    public ChangeOperation Operation => string.IsNullOrEmpty(Op)
        ? ChangeOperation.Unknown
        : Op.ToLowerInvariant() switch
        {
            "c" => ChangeOperation.Create,
            "u" => ChangeOperation.Update,
            "d" => ChangeOperation.Delete,
            _ => ChangeOperation.Unknown
        };

    public Dictionary<string, TrackFieldChange> GetChangedFields()
    {
        var changes = new Dictionary<string, TrackFieldChange>();

        if (Before == null || After == null)
            return changes;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var oldValue = prop.GetValue(Before);
            var newValue = prop.GetValue(After);

            if (!Equals(oldValue, newValue))
            {
                changes[prop.Name] = new TrackFieldChange { OldValue = oldValue, NewValue = newValue };
            }
        }

        return changes;
    }
}
public class TrackFieldChange
{
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
