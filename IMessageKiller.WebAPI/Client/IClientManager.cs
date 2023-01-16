namespace IMessageKiller.WebAPI.Client;

public interface IClientManager
{
    IEnumerable<ClientConnection> Connections { get; }
    Task<ClientConnection> CreateAsync(HttpContext context);
    void Remove(ClientConnection connection);
}