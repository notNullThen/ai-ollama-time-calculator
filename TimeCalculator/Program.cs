using TimeCalculator.Components;
using TimeCalculator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IConsoleLogger, ConsoleLogger>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Use(
        async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/_blazor"))
            {
                app.Logger.LogInformation(
                    "/_blazor request: {Method} {Path} (Upgrade={Upgrade})",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers.Upgrade.ToString()
                );
            }

            await next(context);
        }
    );
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
