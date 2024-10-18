using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    private static List<WebSocket> _clients = new List<WebSocket>();

    public static async Task Handle(HttpContext context, WebSocket webSocket)
    {
        _clients.Add(webSocket);

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastMessage(message);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _clients.Remove(webSocket);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public static async Task BroadcastMessage(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var client in _clients)
        {
            if (client.State == WebSocketState.Open)
            {
              //  var bytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }  
        }
    }
}