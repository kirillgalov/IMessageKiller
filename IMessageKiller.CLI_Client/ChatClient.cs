using System.Net.WebSockets;
using IMessageKiller.SharedLib;

namespace IMessageKiller.CLI_Client;

public class ChatClient : IDisposable
{
    private const int BufferSize = 1028 * 8;
    public ClientWebSocket Socket { get; } = new ClientWebSocket();
    public Uri? Uri { get; private set; }

    public event Action<Message>? MessageReceived;

    public async Task ConnectAsync(string address)
    {
        Uri = new Uri($"ws://{address}/connect");
        await Socket.ConnectAsync(Uri, CancellationToken.None);
        StartListen();
    }

    private void StartListen()
    {
        _ = Task.Run(async () =>
        {
            var buffer = new ArraySegment<byte>(new byte[BufferSize]);
            try
            {
                while (Socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult received = await Socket.ReceiveAsync(buffer, CancellationToken.None);
                    if (received.MessageType == WebSocketMessageType.Text)
                    {
                        var messages = Utils.ReadFromBuffer<List<Message>>(buffer, received.Count);
                        try
                        {
                            foreach (Message message in messages) 
                                OnMessageReceived(message);
                        }
                        catch (Exception) { /* ignored */ }
                    }
                    else if (received.MessageType == WebSocketMessageType.Close)
                    {
                        await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "see you", CancellationToken.None);
                    }
                }        
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        });
    }

    private void OnMessageReceived(Message message)
    {
        MessageReceived?.Invoke(message);
    }

    public async Task SendAsync(string text, string nickname)
    {
        var message = new Message()
        {
            Text = text,
            AuthorNickname = nickname,
            DateTime = DateTime.UtcNow,
        };
        await Socket.SendAsync(Utils.ToBytes(message), 
            WebSocketMessageType.Text, 
            WebSocketMessageFlags.None,
            CancellationToken.None);
    }

    public Task CloseAsync()
    {
        return Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
    }

    public void Dispose()
    {
        Socket.Dispose();
    }
}
