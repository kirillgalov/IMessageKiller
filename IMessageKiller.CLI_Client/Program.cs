using IMessageKiller.CLI_Client;

string nickName = GetArg(args, "--nickname", "User" + DateTime.Now.Second);
string address = GetArg(args, "--address", "127.0.0.1:8080");

Console.WriteLine($"Hello, {nickName}!");

using MessagePrinter messagePrinter = new MessagePrinter();
messagePrinter.StartPrintingThread();

using ChatClient chatClient = new ChatClient();

chatClient.MessageReceived += message =>
{
    if (message.AuthorNickname == nickName)
        return;
    // ReSharper disable once AccessToDisposedClosure
    messagePrinter.AddForPrint(message);
}; 

Console.WriteLine($"Trying to connect... {address}");
try
{
    await chatClient.ConnectAsync(address);
}
catch (Exception)
{
    Console.WriteLine($"Cannot connect to {address}");
    return;
}


Console.WriteLine("Successfully connected");

while (true)
{
    string? msg = Console.ReadLine();
    if (msg == "exit")
    {
        await chatClient.CloseAsync();
        break;
    }
    if (string.IsNullOrEmpty(msg))
        continue;
    
    await chatClient.SendAsync(msg, nickName);
}

Console.WriteLine("Connection was closed");

static string GetArg(string[] args, string argName, string def)
{
    return args.SkipWhile(arg => arg != argName).Skip(1).Take(1).FirstOrDefault(def);
}