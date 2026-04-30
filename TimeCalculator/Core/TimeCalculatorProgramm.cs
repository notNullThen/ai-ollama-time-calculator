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
    public int RelativeHours { get; set; }
    public int RelativeMinutes { get; set; }

    public void SetType(TimeType type)
    {
        CurrentTimeEntry.Type = type;
    }

    public void ReplaceEntryWithCurrent(Guid guid)
    {
        var index = TimeEntries.FindIndex(e => e.Id == guid);
        if (index != -1)
        {
            CurrentTimeEntry.Id = guid;
            TimeEntries[index] = CurrentTimeEntry;
        }
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

    public void SetRelativeHours(int hours) => RelativeHours = hours;

    public void SetRelativeMinutes(int minutes) => RelativeMinutes = minutes;

    public void SetDescription(string description) => CurrentTimeEntry.Description = description;

    public void AddTimeEntry()
    {
        CalculateRelativeTime();

        CurrentTimeEntry.Id = Guid.NewGuid();
        TimeEntries.Add(CurrentTimeEntry);

        CalculateTime();
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

    private void CalculateRelativeTime()
    {
        if (RelativeHours != 0 || RelativeMinutes != 0)
        {
            var lastTime = TimeEntries.Count > 0 ? TimeEntries.Last().Time : TimeSpan.Zero;
            CurrentTimeEntry.Time =
                lastTime
                + TimeSpan.FromHours(RelativeHours)
                + TimeSpan.FromMinutes(RelativeMinutes);
            RelativeHours = 0;
            RelativeMinutes = 0;
        }
    }

    private TimeSpan GetTimeLeftToWork() => TotalWorkTime - TimeSpan.FromHours(DailyWorkHours);
}
