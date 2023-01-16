using IMessageKiller.WebAPI.Client;
using Microsoft.AspNetCore.Mvc;

namespace IMessageKiller.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IClientManager _clientManager;

    public ChatController(ILogger<ChatController> logger, IClientManager clientManager)
    {
        _logger = logger;
        _clientManager = clientManager;
    }


    [Route("/connect")]
    public async Task Connect()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using ClientConnection client = await _clientManager.CreateAsync(HttpContext);
        _logger.LogInformation($"Connected client: {client.Id}");
        try
        {
            await client.ReceiveAsync(HttpContext.RequestAborted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"Connection was aborted: {client.Id}");
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "error on receiving");
        }

        _clientManager.Remove(client);
        _logger.LogInformation($"Disconnected client: {client.Id}");
    }
    
}