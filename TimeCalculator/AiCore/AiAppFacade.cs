using AIOrchestrator.Core.AiAppFacade;
using AIOrchestrator.Core.AiAppFacade.Types;
using TimeCalculator.AiCore.Types;
using TimeCalculator.Core;
using TimeCalculator.Core.Types;

namespace TimeCalculator.AiCore;

public sealed class AiAppFacade(TimeCalculatorProgramm timeCalculator) : AiAppFacadeBase
{
    // Architecture Note: I intentionally use multiple fine-grained functions
    // to achieve the goal instead of a lower number of functions.
    // This demonstrates the AIOrchestrator ability to orchestrate complex logic
    // and execute multi-step sequences.

    public void SetDaytimeHours(string hours)
    {
        timeCalculator.SetHours(int.Parse(hours));
    }

    public void SetDaytimeMinutes(string minutes)
    {
        timeCalculator.SetMinutes(int.Parse(minutes));
    }

    public void SetDurationHours(string hours)
    {
        timeCalculator.SetDurationHours(int.Parse(hours));
    }

    public void SetDurationMinutes(string minutes)
    {
        timeCalculator.SetDurationMinutes(int.Parse(minutes));
    }

    public void SetType(string type)
    {
        timeCalculator.SetType(Enum.Parse<TimeType>(type));
    }

    public void SetDescription(string description)
    {
        timeCalculator.SetDescription(description);
    }

    public AiTimeEntry[] AddTimeEntry()
    {
        timeCalculator.AddTimeEntry();
        return GetTimeEntriesTable();
    }

    public void SetRemainedTime()
    {
        timeCalculator.SetRemainedTime();
    }

    public override string GetConstraints() =>
        @$"
# CORE RULE
You are filling the working day time report.
Understand the user request as a working day sequence of activities which have specific start times, durations, and descriptions.
Each entry MUST be completed by calling {nameof(AddTimeEntry)} before the next begins.

# ENTRY FLOW
Each entry follows:
({nameof(SetType)}) → ({nameof(SetDaytimeHours)}/{nameof(SetDaytimeMinutes)} OR {nameof(SetDurationHours)}/{nameof(SetDurationMinutes)} OR {nameof(SetRemainedTime)}) → ({nameof(SetDescription)}) → ({nameof(AddTimeEntry)})

# STRICT RULES
1. You MUST close an open entry with {nameof(AddTimeEntry)}.
2. You MUST NOT call {nameof(SetType)} before closing the previous entry.
3. You MUST treat one segment as one entry.
4. You MUST use {nameof(SetRemainedTime)} then {nameof(AddTimeEntry)} if the last segment has no duration.
";

    public override AppDescription GetDescription() =>
        [
            new()
            {
                Name = nameof(SetDaytimeHours),
                Description =
                    "Set hours for the current entry (0-23). Provide only the value. Use it when user specifies the time of the day in 24-hour format when activity starts (or ends which means this is the start of the next activity).",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetDaytimeMinutes),
                Description =
                    "Set minutes for the current entry (0-59). Provide only the value. Use it when user specifies the exact time of the day in 24-hour format.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetDurationHours),
                Description =
                    "Set duration in hours for the entry. Use it when user specifies activity the duration time.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetDurationMinutes),
                Description =
                    "Set duration in minutes for the entry. Use it when user specifies activity the duration time.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetType),
                Description = "Start a new entry. Values: Work, Break, or DayEnd.",
                Parameters = [new() { Name = "value", Description = "string" }],
            },
            new()
            {
                Name = nameof(SetDescription),
                Description =
                    "Set short description for the current entry - use all additional information except time and type here, rewrite it and make it professional looking and use it this function.",
                Parameters = [new() { Name = "value", Description = "string" }],
            },
            new()
            {
                Name = nameof(SetRemainedTime),
                Description =
                    "Set remaining work time for the final entry when duration is not specified.",
                Parameters = [],
            },
            new()
            {
                Name = nameof(AddTimeEntry),
                Description =
                    "Close and save the current entry. Must be called after setting time. Returns the updated time table.",
                Parameters = [],
            },
            new()
            {
                Name = "Exit",
                Description = "Finish after all entries are completed.",
                Parameters = [],
            },
        ];

    private AiTimeEntry[] GetTimeEntriesTable() =>
        timeCalculator
            .TimeEntries.Select(entry => new AiTimeEntry
            {
                Time = entry.Time.ToFormattedString(),
                Duration = entry.Duration.ToFormattedString(),
                Type = entry.Type.ToString(),
                Description = entry.Description,
            })
            .ToArray();
}
