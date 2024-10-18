using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using tooms.data;
using tooms.dtos.user;
using tooms.mappers;
using tooms.models;
using tooms.Services;

namespace tooms.controllers
{
    [ApiController]
    [Route("ws")]

    public class WsController : ControllerBase
    {

        //private static readonly ConcurrentDictionary<Guid, WebSocket> _connectedClients = new();
        private static readonly ConcurrentDictionary<User, WebSocket> _connectedClients = new();
        private UserService userService;
        private ConversationService conversationService;
        private readonly ApplicationDBContext context;

        public WsController(UserService userService, ConversationService conversationService, ApplicationDBContext DBcontext)
        {
            this.userService = userService;
            this.conversationService = conversationService;
            this.context = DBcontext;
        }

        [HttpGet()]
        public async Task<IActionResult> Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var connectedCount = _connectedClients.Count();
                Console.WriteLine("Connected count : " + connectedCount);

                var clientId = await userService.GetOne(connectedCount + 1);

                if (clientId != null)
                {
                    Console.WriteLine($"je suis : {clientId.Nickname}");
                    _connectedClients.TryAdd(clientId, socket);
                    await HandleWebSocketCommunication(clientId, socket);
                }


                return Ok();
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }
        }

        private async Task HandleWebSocketCommunication(User clientId, WebSocket socket)
        {
            var buffer = new byte[1024 * 16];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        //Console.WriteLine($"Message reçu de {clientId}: {message}");

                        if (TryParseJson(message, out var parsedMessage))
                        {
                            Console.WriteLine(parsedMessage);
                            if (parsedMessage.TryGetProperty("type", out var typeProperty))
                            {
                                var messageType = typeProperty.GetString();
                                parsedMessage.TryGetProperty("data", out JsonElement jsonData);
                                parsedMessage.TryGetProperty("conversation", out JsonElement conversationId);
                                var conversionId = conversationId.GetInt16();
                                Console.WriteLine($"conversation number {conversationId}");
                                    switch (messageType)
                                {
                                    case "message":
                                        jsonData.TryGetProperty("sender", out JsonElement sender);
                                        jsonData.TryGetProperty("message", out JsonElement msg);
                                        Console.WriteLine(sender.GetInt16() + " : " + msg.GetString());
                                        break;
                                    case "call":
                                        await CallConversation(parsedMessage, clientId, conversionId);
                                        Console.WriteLine($"Call conversation number : {conversationId}");
                                        break;
                                    default:
                                        Console.WriteLine("Type de message inconnu");
                                        break;
                                }
                            }
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"Client {clientId} déconnecté.");
                        _connectedClients.TryRemove(clientId, out _);
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client déconnecté", CancellationToken.None);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Erreur WebSocket : {ex.Message}");
            }
            finally
            {
                _connectedClients.TryRemove(clientId, out _);
                socket.Dispose();
            }
        }

        private async Task CallConversation(JsonElement message, User clientId, int conversationId)
        {
            var conversation = await conversationService.GetOne(conversationId);
            if (conversation == null) return;
            var conversationDto = conversation.ToConversationDto(this.context);
            var users = conversationDto.Users;
            var userIds = users.Select(user => user.Id).ToArray();
            string jsonString = message.GetRawText();
            foreach (var client in _connectedClients)
            {
                if (client.Key.Id != clientId.Id && userIds.Contains(client.Key.Id) && client.Value.State == WebSocketState.Open)
                {
                    await client.Value.SendAsync(
                        Encoding.UTF8.GetBytes(jsonString),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
        }

        private async Task BroadcastMessage(JsonElement message, User clientId)
        {
            string jsonString = message.GetRawText();
            // Relayer le message à tous les autres clients connectés
            foreach (var client in _connectedClients)
            {
                if (client.Key != clientId && client.Value.State == WebSocketState.Open)
                {
                    await client.Value.SendAsync(
                        Encoding.UTF8.GetBytes(jsonString),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
        }

        private bool TryParseJson(string jsonString, out JsonElement parsedMessage)
        {
            try
            {
                parsedMessage = JsonDocument.Parse(jsonString).RootElement;
                return true;
            }
            catch (JsonException)
            {
                parsedMessage = default;
                return false;
            }
        }
    }

}

