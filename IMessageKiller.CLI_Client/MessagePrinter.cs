using System.Collections.Concurrent;
using IMessageKiller.SharedLib;

namespace IMessageKiller.CLI_Client;

public class MessagePrinter : IDisposable
{
    private const int MillisecondsDelay = 10;
    private readonly ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public void AddForPrint(Message message)
    {
        _messages.Enqueue(message);
    }
    
    public void StartPrintingThread()
    {
        _ = Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(MillisecondsDelay);
                if (UserIsStartTypingMessage())
                    continue;
                while (_messages.TryDequeue(out var message)) 
                    Print(message);
            }   
        });
    }

    private void Print(Message message)
    {
        Console.WriteLine($"{message.Text} [{message.DateTime.ToLocalTime():HH:mm}] < {message.AuthorNickname}");
    }

    private bool UserIsStartTypingMessage()
    {
        return Console.CursorLeft != 0;
    }
    
    public void Dispose()
    {
        _cts.Cancel();
    }
}