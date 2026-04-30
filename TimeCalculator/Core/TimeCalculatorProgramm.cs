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
        CalculateTotalTime();
    }

    public void RemoveTimeEntry(Guid guid)
    {
        TimeEntries.RemoveAll(e => e.Id == guid);
        CalculateTotalTime();
    }

    public void SetHours(int hours)
    {
        CurrentTimeEntry.Time += TimeSpan.FromHours(hours);
    }

    public void SetMinutes(int minutes)
    {
        CurrentTimeEntry.Time += TimeSpan.FromMinutes(minutes);
    }

    public void SetSeconds(int seconds)
    {
        CurrentTimeEntry.Time += TimeSpan.FromSeconds(seconds);
    }

    public void SetDescription(string description)
    {
        CurrentTimeEntry.Description = description;
    }

    public void AddTimeEntry()
    {
        CurrentTimeEntry.Id = Guid.NewGuid();
        TimeEntries.Add(CurrentTimeEntry);

        CalculateTotalTime();
        CurrentTimeEntry = new();
    }

    public void SetRemainedTime()
    {
        CurrentTimeEntry = new()
        {
            Time = -TotalTimeLeftToWork,
            Type = CurrentTimeEntry.Type,
            Description = CurrentTimeEntry.Description,
        };
    }

    public void CalculateTotalTime()
    {
        var workTimeEntries = TimeEntries
            .Where(timeEntry => timeEntry.Type == TimeType.Work)
            .ToList();

        TotalTime = SumTimeEntries(TimeEntries);
        TotalWorkTime = SumTimeEntries(workTimeEntries);
    }

    private TimeSpan GetTimeLeftToWork() => TotalWorkTime - TimeSpan.FromHours(DailyWorkHours);

    private TimeSpan SumTimeEntries(IEnumerable<TimeEntry> entries)
    {
        var totalTime = new TimeSpan();

        foreach (var entry in entries)
        {
            totalTime += entry.Time;
        }

        return totalTime;
    }
}
