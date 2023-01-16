namespace IMessageKiller.WebAPI.Client;

public readonly struct ConnectionId : IEquatable<ConnectionId>
{
    private readonly string _id;

    public ConnectionId(string id) => _id = id;

    public bool Equals(ConnectionId other) => _id == other._id;

    public override bool Equals(object? obj) => obj is ConnectionId other && Equals(other);

    public override int GetHashCode() => _id.GetHashCode();

    public override string ToString() => _id;
}