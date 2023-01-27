using Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Clients;

var app = Startup.Build(args);

Handlers.Logger = app.Logger;
Handlers.GameClient = app.Services.GetRequiredService<IGameClient>();

app.MapGet("/", Handlers.Default);
app.MapGet("/games/{language}/{questionSelectionStrategy}", Handlers.StartGame);

app.Run();