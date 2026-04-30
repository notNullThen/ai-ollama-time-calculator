namespace TimeCalculator.Core;

public static class TimeSpanExtensions
{
    private const string TimeFormat = @"hh\:mm";

    public static string ToFormattedString(this TimeSpan time) => time.ToString(TimeFormat);

    public static bool IsZero(this TimeSpan time) => time == TimeSpan.Zero;
}
