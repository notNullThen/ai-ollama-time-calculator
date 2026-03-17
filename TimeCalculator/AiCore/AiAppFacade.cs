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

    public void SetHours(int hours) => timeCalculator.SetHours(hours);
    public void SetMinutes(int minutes) => timeCalculator.SetMinutes(minutes);
    public void SetSeconds(int seconds) => timeCalculator.SetSeconds(seconds);
    public void SetType(TimeType type) => timeCalculator.SetType(type);
    public void WriteTimeEntryToTable() => timeCalculator.AddTimeEntry();

    public override string GetConstraints() => @$"
Define which functions to call and in which order based on user input.

Usual workflow:

1. You set the type of the time entry
2. You set the time (hours, minutes, seconds)
3. You write the time entry to the table
4. Repeat steps 1-3 for each time entry until user's input is satisfied
5. When user input is satisfied, call the {nameof(Exit)} function.
";

    public override AppDescription GetDescription() =>
    [
        new()
        {
            Name = nameof(SetHours),
            Description = $@"
Sets hours in current time entry. Set 0 if not specified in user request.
",
            Parameters =
            [
                new()
                {
                    Name = "hours",
                    Description = @"
Type: int.
Format: 0, 1, 2, ... 23.
"
                }
            ]
        },
        new()
        {
            Name = nameof(SetMinutes),
            Description = $@"
Set minutes in current time entry. Set 0 if not specified in user request.
",
            Parameters =
            [
                new()
                {
                    Name = "minutes",
                    Description = @"
Type: int.
Format: 0, 1, 2, ... 59.
"
                }
            ]
        },
        new()
        {
            Name = nameof(SetSeconds),
            Description = $@"
Set seconds in current time entry. Set 0 if not specified in user request.
",
            Parameters =
            [
                new()
                {
                    Name = "seconds",
                    Description = @"
Type: int.
Format: 0, 1, 2, ... 59.
"
                }
            ]
        },
        new()
        {
            Name = nameof(SetType),
            Description = "Set type in current time entry. Define type from the user request.",
            Parameters =
            [
                new()
                {
                    Name = "type",
                    Description = @"
Type: string.
Format: ""Work"" or ""Break"".
"
                }
            ]
        },
        new() { Name = nameof(WriteTimeEntryToTable), Description = "Write time entry to table.", Parameters = [] }
    ];
}