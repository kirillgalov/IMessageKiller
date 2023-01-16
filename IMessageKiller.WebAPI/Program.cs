using IMessageKiller.WebAPI.Client;
using IMessageKiller.WebAPI.HostedSevices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostedService<MessageBroadcastService>();
builder.Services.AddSingleton<IClientManager, ClientManager>();

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromSeconds(10) });
app.MapControllers();

app.Run("http://*:8080");