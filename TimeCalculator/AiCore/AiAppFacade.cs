using System.Text.Json;
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

    private static readonly JsonSerializerOptions PrettyJsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    public void SetDuration(string hours) => timeCalculator.SetDurationHours(int.Parse(hours));

    public Guid AddTimeEntry(string time, string type, string description)
    {
        SetTime(time);
        SetType(type);
        timeCalculator.SetDescription(description);

        return timeCalculator.AddTimeEntry();
    }

    public Guid ReplaceEntry(string guid, string time, string type, string description)
    {
        var parsedGuid = Guid.Parse(guid);

        SetTime(time);
        SetType(type);
        timeCalculator.SetDescription(description);

        timeCalculator.ReplaceEntryWithCurrent(parsedGuid);
        return parsedGuid;
    }

    public void EndTheDay()
    {
        timeCalculator.SetType(TimeType.DayEnd);
        timeCalculator.SetDescription("Departure from work.");
        timeCalculator.SetRemainedTime();
        timeCalculator.AddTimeEntry();
    }

    public override string GetConstraints() =>
        @$"
You are filling the working day time report.
Understand the user request as a working day sequence of activities which have specific start times, durations, and descriptions.

Current time entries table:
{GetTimeEntriesTable()}
";

    public override AppDescription GetDescription() =>
        [
            new()
            {
                Name = nameof(AddTimeEntry),
                Description =
                    "Adds new time entry with specified time and type. Returns the id of the created entry.",
                Parameters =
                [
                    new() { Name = "time", Description = "Time in format HH:mm" },
                    new()
                    {
                        Name = "type",
                        Description =
                            $"Type of the time entry, can be only {nameof(TimeType.Work)}, {nameof(TimeType.Break)}.",
                    },
                    new()
                    {
                        Name = "description",
                        Description =
                            "Short description for the current entry. Use all additional information except time. Make this description to look officialy-professional before using it as parameter.",
                    },
                ],
            },
            new()
            {
                Name = nameof(ReplaceEntry),
                Description =
                    "Replaces existing entry with the new time, type and description. Returns the id of the replaced entry.",
                Parameters =
                [
                    new() { Name = "guid", Description = "Id of the entry to replace." },
                    new() { Name = "time", Description = "Time in format HH:mm" },
                    new()
                    {
                        Name = "type",
                        Description =
                            $"Type of the time entry, can be only {nameof(TimeType.Work)}, {nameof(TimeType.Break)}.",
                    },
                    new()
                    {
                        Name = "description",
                        Description =
                            "Short description for the current entry. Use all additional information except time. Make this description to look officialy-professional before using it as parameter.",
                    },
                ],
            },
            new()
            {
                Name = nameof(EndTheDay),
                Description =
                    "Sets remaining time to the end of the working day and adds the final entry with type DayEnd.",
                Parameters = [],
            },
        ];

    private void SetTime(string time)
    {
        timeCalculator.CurrentTimeEntry.Time = TimeSpan.ParseExact(
            input: time,
            format: @"hh\:mm",
            formatProvider: null
        );
    }

    private void SetType(string type) => timeCalculator.SetType(Enum.Parse<TimeType>(type));

    private string GetTimeEntriesTable()
    {
        var aiTimeEntries = timeCalculator
            .TimeEntries.Select(entry => new AiTimeEntry
            {
                Id = entry.Id.ToString(),
                Time = entry.Time.ToString(@"hh\:mm"),
                Duration = entry.Duration.ToString(@"hh\:mm"),
                Type = entry.Type.ToString(),
                Description = entry.Description,
            })
            .ToArray();

        return JsonSerializer.Serialize(aiTimeEntries, PrettyJsonSerializerOptions);
    }
}
