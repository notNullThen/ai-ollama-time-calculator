using System.Text.Json.Serialization;

namespace TimeCalculator.Core.Types;

public class TimeEntry
{
    [JsonPropertyName("timeEntry")]
    public TimeSpan Time { get; set; } = new();
    public TimeType Type { get; set; } = TimeType.Work;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
