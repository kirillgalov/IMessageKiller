using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace IMessageKiller.WebAPI.Client;

public class ClientManager : IClientManager
{
    private readonly ConcurrentDictionary<ConnectionId, ClientConnection> _connections = new();

    public IEnumerable<ClientConnection> Connections => _connections.Values;

    public async Task<ClientConnection> CreateAsync(HttpContext context)
    {
        WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
        var connection = new ClientConnection
        {
            Id = new ConnectionId(context.Connection.Id),
            Socket = socket,
        };
        _connections.TryAdd(connection.Id, connection);
        return connection;
    }

    public void Remove(ClientConnection connection)
    {
        _connections.TryRemove(connection.Id, out _);
    }
}