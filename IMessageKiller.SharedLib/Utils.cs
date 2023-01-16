using System.Text;
using System.Text.Json;

namespace IMessageKiller.SharedLib;

public class Utils
{
    public static byte[] ToBytes<T>(T obj)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
    }

    public static T ReadFromBuffer<T>(ArraySegment<byte> buffer, int bytesCount)
    {
        string json = Encoding.UTF8.GetString(buffer[..bytesCount]);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}