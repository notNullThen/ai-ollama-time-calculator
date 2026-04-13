# AI Time Manager

> **Architecture Note:** I intentionally use multiple fine-grained functions to achieve the goal instead of a lower number of functions. This demonstrates the AIOrchestrator ability to orchestrate complex logic and execute multi-step sequences.

### A basic concept of Time Data Management application with **AI assistance** functionality built with **.NET 10** and **Blazor**.

Runs on Ollama - gemma4:e2b.

### Uses own developed [AIOrchestrator NuGet package](https://github.com/notNullThen/AIOrchestratorDotNET).

<img src="assets/screenshot.png" alt="App Screenshot" width="600" />

> **Note:** The C# logic is implemented manually, while the frontend-related code (HTML, CSS, etc) is vibe-coded. The AI Orchestration is powered by the own developed [AIOrchestrator NuGet package](https://github.com/notNullThen/AIOrchestratorDotNET).

The web application processes human input and fills the time data.

[AIOrchestrator](https://github.com/notNullThen/AIOrchestratorDotNET) has access to `SetHours(int)`, `SetMinutes(int)`, `SetSeconds(int)`, `SetType(Work/Break)`, `SetRemainedTime()` and `AddTimeEntry()` methods.

## AI Assistant Setup

The AI features are powered by [Ollama](https://ollama.com/). By default, it expects the `gemma4:e2b` model.

1.  **Install Ollama:** Follow instructions at [ollama.com](https://ollama.com/).
2.  **Pull the model:**
    ```bash
    ollama pull gemma4:e2b
    ```
3.  **Run the application**

## Run

### Standard run (recommended):
```bash
dotnet run --project TimeCalculator
```

### Run with network access (accessible from other devices, doesn't support AI yet):
```bash
dotnet run --project TimeCalculator --urls "http://0.0.0.0:8080"
```

### Run with Docker Compose (doesn't support AI yet):
```bash
docker compose up --build
```

The application will be available at `http://localhost:8080` by default. The `--build` flag ensures the Docker image is rebuilt with the latest changes from your local codebase.
