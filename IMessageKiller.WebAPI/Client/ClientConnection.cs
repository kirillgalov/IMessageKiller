using System.Collections.Concurrent;
using System.Net.WebSockets;
using IMessageKiller.SharedLib;

namespace IMessageKiller.WebAPI.Client;

public class ClientConnection : IDisposable
{
    private const int BufferSize = 1024 * 8;
    public required WebSocket Socket { get; init; } = null!;
    public required ConnectionId Id { get; init; }
    public ConcurrentQueue<Message> Received { get; } = new();

    public async Task ReceiveAsync(CancellationToken cancellationToken)
    {
        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[BufferSize]);
        while (Socket.State == WebSocketState.Open)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            WebSocketReceiveResult receiveResult = await Socket.ReceiveAsync(buffer, cancellationToken);
            
            if (receiveResult.MessageType == WebSocketMessageType.Text)
            {
                var message = Utils.ReadFromBuffer<Message>(buffer, receiveResult.Count);
                Received.Enqueue(message);
            } 
            else if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "see you", cancellationToken);
                return;
            }
        }
    }

    public async Task SendAsync(List<Message> messages, CancellationToken cancellationToken)
    {
        if (Socket.State != WebSocketState.Open)
            return;

        await Socket.SendAsync(Utils.ToBytes(messages), 
            WebSocketMessageType.Text,
            WebSocketMessageFlags.None, 
            cancellationToken);
    }

    public void Dispose()
    {
        Socket.Dispose();
    }
}