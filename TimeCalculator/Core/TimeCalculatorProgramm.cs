using TimeCalculator.Core.Types;

namespace TimeCalculator.Core;

public class TimeCalculatorProgramm
{
    public Dictionary<Guid, TimeEntry> TimeEntries = [];

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
        TimeEntries[guid] = CurrentTimeEntry;
        CalculateTotalTime();
    }

    public void RemoveTimeEntry(Guid guid)
    {
        TimeEntries.Remove(guid);
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
        TimeEntries.Add(Guid.NewGuid(), CurrentTimeEntry);

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
            .Where(timeEntry => timeEntry.Value.Type == TimeType.Work)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        TotalTime = SumTimeEntries(TimeEntries);
        TotalWorkTime = SumTimeEntries(workTimeEntries);
    }

    private TimeSpan GetTimeLeftToWork() => TotalWorkTime - TimeSpan.FromHours(DailyWorkHours);

    private TimeSpan SumTimeEntries(Dictionary<Guid, TimeEntry> entries)
    {
        var totalTime = new TimeSpan();

        foreach (var entry in entries)
        {
            totalTime += entry.Value.Time;
        }

        return totalTime;
    }
}
