using AIOrchestrator.Core;
using AIOrchestrator.Core.Types;
using TimeCalculator.Core;
using TimeCalculator.Services;

namespace TimeCalculator.AiCore;

public class AiInteraction
{
    public AiManager? AiManager { get; private set; }

    public string UserInput { get; set; }

    private readonly AiAppFacade _aiFacade;
    private readonly IConsoleLogger _logger;

    public event EventHandler<List<FunctionCallResponse>>? OnContextUpdated;
    public event EventHandler? OnBusyChanged;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnBusyChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public AiInteraction(TimeCalculatorProgramm timeCalculator, IConsoleLogger logger)
    {
        _aiFacade = new AiAppFacade(timeCalculator);
        _logger = logger;
        UserInput = string.Empty;
        Init();
    }

    private const string ModelName = "gemma4:e4b";

    public async Task AskAsync()
    {
        if (IsBusy)
            return;
        IsBusy = true;
        try
        {
            await AiManager!.StartAsync(UserInput);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"AI Error: {ex.Message}", ex);
            throw;
        }
    }

    public string GetContext() => AiManager!.ContextHandler.GetContextJson();

    public string GetManagementPrompt() => AiManager!.GetManagementPrompt();

    public void Init()
    {
        if (AiManager?.ContextHandler != null)
        {
            AiManager.ContextHandler.OnContextUpdated -= InternalOnContextUpdated;
        }

        AiManager = new(
            modelName: ModelName,
            appInstance: _aiFacade,
            options: new() { Temperature = 0.0f }
        );
        _aiFacade.OnExit = () => IsBusy = false;
        AiManager.ContextHandler.OnContextUpdated += InternalOnContextUpdated;
        UserInput = string.Empty;
    }

    private void InternalOnContextUpdated(object? sender, List<FunctionCallResponse> e)
    {
        OnContextUpdated?.Invoke(this, e);
    }
}
