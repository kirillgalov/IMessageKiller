using IMessageKiller.SharedLib;
using IMessageKiller.WebAPI.Client;

namespace IMessageKiller.WebAPI.HostedSevices;

public class MessageBroadcastService : BackgroundService
{
    private const int BroadcastMillisecondsDelay = 200;
    private readonly ILogger<MessageBroadcastService> _logger;
    private readonly IClientManager _clientManager;

    public MessageBroadcastService(ILogger<MessageBroadcastService> logger, IClientManager clientManager)
    {
        _logger = logger;
        _clientManager = clientManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var messagesToSend = new List<Message>();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(BroadcastMillisecondsDelay, stoppingToken);
            messagesToSend.Clear();

            foreach (ClientConnection client in _clientManager.Connections)
            {
                while (client.Received.TryDequeue(out var msg)) 
                    messagesToSend.Add(msg);
            }
            
            if (messagesToSend.Count == 0)
                continue;
            
            foreach (ClientConnection connection in _clientManager.Connections)
            {
                try
                {
                    await connection.SendAsync(messagesToSend, stoppingToken);
                    _logger.LogInformation($"Sent to {connection.Id}");
                }
                catch (Exception e)
                {
                    _logger.LogDebug(e, "error on sending");
                }
            }
        }
    }
}