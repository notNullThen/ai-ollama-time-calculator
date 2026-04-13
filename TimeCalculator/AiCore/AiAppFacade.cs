using AIOrchestrator.Core.AiAppFacade;
using AIOrchestrator.Core.AiAppFacade.Types;
using TimeCalculator.Core;
using TimeCalculator.Core.Types;

namespace TimeCalculator.AiCore;

public sealed class AiAppFacade(TimeCalculatorProgramm timeCalculator) : AiAppFacadeBase
{
    // For demonstration purposes I decided to test the AI's ability to handle
    // complex logic with multiple function calls.
    // Otherwise we would have only one function call to set the time entry.

    public string SetHours(string hours)
    {
        timeCalculator.SetHours(int.Parse(hours));
        return ReturnTempSuccess();
    }

    public string SetMinutes(string minutes)
    {
        timeCalculator.SetMinutes(int.Parse(minutes));
        return ReturnTempSuccess();
    }

    public string SetSeconds(string seconds)
    {
        timeCalculator.SetSeconds(int.Parse(seconds));
        return ReturnTempSuccess();
    }

    public string SetType(string type)
    {
        timeCalculator.SetType(Enum.Parse<TimeType>(type));
        return ReturnTempSuccess();
    }

    public TimeEntry[] AddTimeEntry()
    {
        timeCalculator.AddTimeEntry();
        return GetTimeEntriesTable();
    }

    public TimeEntry[] SetRemainedTime()
    {
        timeCalculator.SetRemainedTime();
        return AddTimeEntry();
    }

    public override string GetConstraints() =>
        @$"
# CORE RULE
Process the user request as a sequence of entries.
Each entry MUST be completed before the next begins.

# ENTRY FLOW
Each entry follows:
({nameof(SetType)}) → (set time OR {nameof(SetRemainedTime)}) → ({nameof(AddTimeEntry)})

Repeat this flow for every segment.

# STRICT RULES

- After {nameof(SetType)} the entry is OPEN
- An OPEN entry MUST be closed with {nameof(AddTimeEntry)}
- NEVER call {nameof(SetType)} before closing the previous entry
- {nameof(SetType)} is ONLY allowed after {nameof(AddTimeEntry)}

- One segment = one entry
- DO NOT merge or skip segments
- DO NOT change an entry once started

- If last segment has no duration → use {nameof(SetRemainedTime)} then {nameof(AddTimeEntry)}

# OUTPUT FORMAT

Return ONE JSON object:
""Function"": string
""Parameters"": string[]
";

    public override AppDescription GetDescription() =>
        [
            new()
            {
                Name = nameof(SetHours),
                Description = "Set hours for the current entry (0-23). Provide only the value.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetMinutes),
                Description = "Set minutes for the current entry (0-59). Provide only the value.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetSeconds),
                Description = "Set seconds for the current entry (0-59). Provide only the value.",
                Parameters = [new() { Name = "value", Description = "int" }],
            },
            new()
            {
                Name = nameof(SetType),
                Description = "Start a new entry. Values: Work or Break.",
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
                    "Close and save the current entry. Must be called after setting time.",
                Parameters = [],
            },
            new()
            {
                Name = "Exit",
                Description = "Finish after all entries are completed.",
                Parameters = [],
            },
        ];

    private TimeEntry[] GetTimeEntriesTable() => [.. timeCalculator.TimeEntries.Values];

    private string ReturnTempSuccess() => "Temporarily set";
}
