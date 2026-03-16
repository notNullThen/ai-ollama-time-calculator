using AIOrchestrator.Core;

using TimeCalculator.Core;

namespace TimeCalculator.AiCore;

public class AiInteraction
{
    public AiManager AiManager;

    public string UserInput { get; set; }


    public AiInteraction(TimeCalculatorProgramm timeCalculator)
    {
        var aiFacade = new AiAppFacade(timeCalculator);
        AiManager = new(modelName: _modelName, appInstance: aiFacade);
        UserInput = string.Empty;
    }


    private const string _modelName = "qwen2.5-coder:7b";


    public async Task AskAsync() => await AiManager.StartAsync(UserInput);

    public string GetContext() => AiManager.ContextHandler.GetContextJson();
}
