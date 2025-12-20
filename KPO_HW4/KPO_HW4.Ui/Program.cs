using Blazored.LocalStorage;
using Blazored.Toast;
using KPO_HW4.Ui.Components;
using KPO_HW4.Ui.Infrastructure;
using KPO_HW4.Ui.Services;
using LumexUI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddBlazoredToast()
    .AddBlazoredLocalStorage()
    .AddLumexServices();

builder.AddInfrastructure();

builder.Services.AddScoped<IUserContext, UserContext>();

var app = builder.Build();
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
