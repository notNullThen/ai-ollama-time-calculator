using Microsoft.JSInterop;

namespace TimeCalculator.Services;

public interface IConsoleLogger
{
    ValueTask LogErrorAsync(string message, Exception? ex = null);
    ValueTask LogWarningAsync(string message);
    ValueTask LogInfoAsync(string message);
    IReadOnlyList<string> Logs { get; }
    void Clear();
}

public class ConsoleLogger(IJSRuntime jsRuntime) : IConsoleLogger
{
    private readonly List<string> _logs = new();
    public IReadOnlyList<string> Logs => _logs.AsReadOnly();

    public async ValueTask LogErrorAsync(string message, Exception? ex = null)
    {
        var logEntry = $"[Error] {message}";
        if (ex != null)
        {
            logEntry += $"\nException: {ex}";
        }
        _logs.Add(logEntry);
        await jsRuntime.InvokeVoidAsync("tcLogger.logError", message, ex?.ToString());
    }

    public async ValueTask LogWarningAsync(string message)
    {
        _logs.Add($"[Warning] {message}");
        await jsRuntime.InvokeVoidAsync("tcLogger.logWarning", message);
    }

    public async ValueTask LogInfoAsync(string message)
    {
        _logs.Add($"[Info] {message}");
        await jsRuntime.InvokeVoidAsync("tcLogger.logInfo", message);
    }

    public void Clear() => _logs.Clear();
}
