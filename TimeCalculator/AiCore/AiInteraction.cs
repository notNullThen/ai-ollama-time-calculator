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
    private readonly TimeCalculatorProgramm _timeCalculator;

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
        _timeCalculator = timeCalculator;
        UserInput = string.Empty;
        Init();
    }


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
        finally
        {
            IsBusy = false;
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
            modelName: _timeCalculator.AiSettings.ModelName,
            appInstance: _aiFacade,
            options: new() { Temperature = 0.0f },
            ollamaBaseUrl: _timeCalculator.AiSettings.BaseUrl
        );
        _aiFacade.OnExit = () => IsBusy = false;
        AiManager.ContextHandler.OnContextUpdated += InternalOnContextUpdated;
    }

    private void InternalOnContextUpdated(object? sender, List<FunctionCallResponse> e)
    {
        OnContextUpdated?.Invoke(this, e);
    }
}
