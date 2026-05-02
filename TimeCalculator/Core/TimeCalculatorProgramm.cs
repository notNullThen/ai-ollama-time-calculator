using TimeCalculator.Core.Types;

namespace TimeCalculator.Core;

public class TimeCalculatorProgramm
{
    public List<TimeEntry> TimeEntries = [];

    public TimeSpan TotalTime = new();
    public TimeSpan TotalWorkTime = new();
    public TimeSpan TotalTimeLeftToWork => GetTimeLeftToWork();
    public TimeEntry CurrentTimeEntry = new();

    public int DailyWorkHours = 0;
    public TimeSpan Duration { get; set; }
    public AiSettings AiSettings { get; set; } = new();

    public void SetType(TimeType type)
    {
        CurrentTimeEntry.Type = type;
    }

    public void ReplaceEntryWithCurrent(Guid guid)
    {
        var index = TimeEntries.FindIndex(e => e.Id == guid);
        if (index == -1)
        {
            throw new Exception($"Entry with guid {guid} not found.");
        }

        var newEntry = CurrentTimeEntry.Clone();
        newEntry.Id = guid;
        TimeEntries[index] = newEntry;

        CalculateTime();
    }

    public void RemoveTimeEntry(Guid guid)
    {
        TimeEntries.RemoveAll(e => e.Id == guid);
        CalculateTime();
    }

    public void SetHours(int hours)
    {
        CurrentTimeEntry.Hours = hours;
    }

    public void SetMinutes(int minutes) => CurrentTimeEntry.Minutes = minutes;

    public void SetDurationHours(int hours)
    {
        Duration = new(hours: hours, minutes: Duration.Minutes, seconds: Duration.Seconds);
    }

    public void SetDurationMinutes(int minutes) =>
        Duration = new(hours: Duration.Hours, minutes: minutes, seconds: Duration.Seconds);

    public void SetDescription(string description) => CurrentTimeEntry.Description = description;

    public Guid AddTimeEntry()
    {
        CalculateDurationTime();

        var newEntry = CurrentTimeEntry.Clone();
        newEntry.Id = Guid.NewGuid();

        TimeEntries.Add(newEntry);
        CalculateTime();
        ResetCurrentTimeEntry();

        return newEntry.Id;
    }

    private void ResetCurrentTimeEntry()
    {
        CurrentTimeEntry = new();
    }

    public void SetRemainedTime()
    {
        var lastEntryTime = TimeEntries.Count > 0 ? TimeEntries.Last().Time : new TimeSpan();

        CurrentTimeEntry = new()
        {
            Time = lastEntryTime - TotalTimeLeftToWork,
            Type = TimeType.DayEnd,
            Description = CurrentTimeEntry.Description,
        };
    }

    public void CalculateTime()
    {
        // Sort entries by time before calculating durations
        TimeEntries = TimeEntries.OrderBy(e => e.Time).ToList();

        TotalTime = new TimeSpan();
        TotalWorkTime = new TimeSpan();

        for (int i = 0; i < TimeEntries.Count; i++)
        {
            var entry = TimeEntries[i];
            TimeSpan duration = TimeSpan.Zero;

            if (i < TimeEntries.Count - 1)
            {
                duration = TimeEntries[i + 1].Time - entry.Time;
            }

            entry.Duration = duration;

            if (entry.Type != TimeType.DayEnd)
            {
                TotalTime += duration;
                if (entry.Type == TimeType.Work)
                {
                    TotalWorkTime += duration;
                }
            }
        }
    }

    private void CalculateDurationTime()
    {
        if (Duration != TimeSpan.Zero)
        {
            var lastTime = TimeEntries.Count > 0 ? TimeEntries.Last().Time : TimeSpan.Zero;
            CurrentTimeEntry.Time = lastTime + Duration;
            Duration = TimeSpan.Zero;
        }
    }

    private TimeSpan GetTimeLeftToWork() => TotalWorkTime - TimeSpan.FromHours(DailyWorkHours);
}
