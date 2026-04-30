using System.Text.Json.Serialization;

namespace TimeCalculator.Core.Types;

public class TimeEntry
{
    [JsonPropertyName("timeEntry")]
    public TimeSpan Time { get; set; } = new();
    public TimeType Type { get; set; } = TimeType.Work;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    public int Hours
    {
        get => Time.Hours;
        set => Time = new TimeSpan(value, Time.Minutes, Time.Seconds);
    }

    public int Minutes
    {
        get => Time.Minutes;
        set => Time = new TimeSpan(Time.Hours, value, Time.Seconds);
    }

    public int Seconds
    {
        get => Time.Seconds;
        set => Time = new TimeSpan(Time.Hours, Time.Minutes, value);
    }
}
