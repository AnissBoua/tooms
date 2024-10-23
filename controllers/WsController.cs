using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using tooms.data;
using tooms.dtos.message;
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
        private readonly MessageService messageService;
        private readonly ApplicationDBContext context;

        public WsController(UserService userService, ConversationService conversationService, MessageService messageService, ApplicationDBContext DBcontext)
        {
            this.userService = userService;
            this.conversationService = conversationService;
            this.messageService = new MessageService(DBcontext);
            this.context = DBcontext;
        }

        [HttpGet()]
        public async Task<IActionResult> Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                Console.WriteLine("Nouvelle connexion WebSocket.");

                // Authentifier et gérer la session WebSocket
                var user = await AuthenticateWebSocket(socket);
                if (user != null)
                {
                    Console.WriteLine($"Utilisateur {user.Nickname} connecté.");
                    _connectedClients.TryAdd(user, socket);
                    // Gérer la communication avec cet utilisateur
                    await HandleWebSocketCommunication(user, socket);
                }
                else
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                        "Authentification échouée", CancellationToken.None);
                }


                return Ok();
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }
        }

        private async Task<User?> AuthenticateWebSocket(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                if (TryParseJson(message, out var parsedMessage) &&
                    parsedMessage.TryGetProperty("token", out var tokenElement))
                {
                    var identifier = tokenElement.GetString();
                    var user = await userService.GetByToken(identifier);
                    if(user != null)
                    {
                        if (user != null)
                        {
                            var userIdJson = JsonSerializer.Serialize(new { type = "userId", data = user.Id });
                            var userIdBytes = Encoding.UTF8.GetBytes(userIdJson);
                            await socket.SendAsync(userIdBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    return user; // Récupère l'utilisateur avec le token
                }
            }
            return null;  // Échec d'authentification
        }

        private async Task HandleWebSocketCommunication(User clientId, WebSocket socket)
        {
            var buffer = new byte[1024 * 16];
          //  var messageService = new MessageService(context);

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
                                Console.WriteLine("Messgae type: " + messageType);
                                switch (messageType)
                                {
                                    case "message":
                                        jsonData.TryGetProperty("sender", out JsonElement sender);
                                        jsonData.TryGetProperty("content", out JsonElement msg);

                                        Console.WriteLine(sender.GetInt32() + " " + msg.GetString());
                                        var userInstance = context.Users.Find(sender.GetInt32());
                                        var conversationInstance = context.Conversations.Find(conversationId.GetInt32());

                                        var messageInstance = new Message
                                        {
                                            User = userInstance,
                                            Conversation = conversationInstance,
                                            Content = msg.GetString(),
                                            CreatedAt = DateTime.UtcNow,
                                            UpdatedAt = DateTime.UtcNow
                                        };

                                        await BroadcastMessageToConversation(conversationInstance, parsedMessage, messageInstance);

                                        await context.Messages.AddAsync(messageInstance);
                                        await context.SaveChangesAsync();

                                        Console.WriteLine(messageInstance.Content);

                                        Console.WriteLine("__________________________________________________________" + sender.GetInt16() + " : " + msg.GetString());
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

        private async Task BroadcastMessageToConversation(Conversation conversation, JsonElement message, Message messageInstance)
        {
            if (conversation == null) return;

            var dto = messageInstance.ToMessageSocket();
            Console.WriteLine(messageInstance.Conversation.ToString() + " " + messageInstance.User.ToString());
            // Convertir messageInstance en JSON string
            var messageInstanceJson = JsonSerializer.Serialize(dto);

            // Convertir JsonElement en JsonObject pour être modifiable
            var jsonDocument = JsonDocument.Parse(message.GetRawText());
            var jsonObject = jsonDocument.RootElement.Clone().Deserialize<Dictionary<string, JsonElement>>();

            // Remplacer la valeur de la clé "data" avec le messageInstance
            if (jsonObject.ContainsKey("data"))
            {
                jsonObject["data"] = JsonDocument.Parse(messageInstanceJson).RootElement;
            }

            // Recréer la chaîne JSON modifiée
            var modifiedJsonString = JsonSerializer.Serialize(jsonObject);
            var conversationDto = conversation.ToConversationDto(this.context);
            var users = conversationDto.Users;
            var userIds = users.Select(user => user.Id).ToArray();

            foreach (var client in _connectedClients)
            {
                if (userIds.Contains(client.Key.Id) && client.Value.State == WebSocketState.Open)
                {
                    await client.Value.SendAsync(
                        Encoding.UTF8.GetBytes(modifiedJsonString),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
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
            Dictionary<string, object>? messageData = jsonString != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString) : null;
            if (messageData == null) return;
            messageData["caller"] = clientId.ToUserDto();
            jsonString = JsonSerializer.Serialize(messageData);

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

