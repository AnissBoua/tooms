using System.Collections.Concurrent;
using System.Net.WebSockets;

public static class WebSocketConnections
{
    public static ConcurrentBag<WebSocket> Clients { get; } = new ConcurrentBag<WebSocket>();

    public static void AddClient(WebSocket webSocket)
    {
        Clients.Add(webSocket);
    }

    public static void RemoveClient(WebSocket webSocket)
    {
        Clients.TryTake(out webSocket);
    }
}