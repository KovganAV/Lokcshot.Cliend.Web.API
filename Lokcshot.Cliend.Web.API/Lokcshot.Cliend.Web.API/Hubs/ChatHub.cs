using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Proof_of_Concept.Models;
using Discord.Channels.Models.Messages.Requests;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Proof_of_Concept.Hubs
{

    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ChatHub : Hub
    {

        private readonly IServersRefit _serverRefit;

        private static readonly ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();

        public ChatHub(IServersRefit serversRefit)
        {

            _serverRefit = serversRefit;

        }

        public async Task JoinChat(UserConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            string serverRoomGroup = $"{connection.Server}:{connection.ChatRoom}";
            
            if (!ConnectedUsers.ContainsKey(serverRoomGroup))
            {
                ConnectedUsers[serverRoomGroup] = new List<string>();
            }

            ConnectedUsers[serverRoomGroup].Add(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, serverRoomGroup);

            var joinMessage = $"{connection.UserName} joined the chat room.";
            await Clients.Group(serverRoomGroup).SendAsync("JoinChat", joinMessage);
        }

        public async Task SendMessage(MessageRequestModel message)
        {

            if (string.IsNullOrWhiteSpace(message.Content)) throw new ArgumentNullException(nameof(message));

            var getMessage = await _serverRefit.PostMessage(message);

            string serverRoomGroup = ConnectedUsers.FirstOrDefault(kvp => kvp.Value.Contains(Context.ConnectionId)).Key;

            await Clients.Group(serverRoomGroup).SendAsync("SendMessage", getMessage);

        }

        public async Task DeleteMessage(Guid messageId)
        {

            string serverRoomGroup = ConnectedUsers.FirstOrDefault(kvp => kvp.Value.Contains(Context.ConnectionId)).Key;

            await _serverRefit.DeleteMessage(messageId);

            await Clients.Group(serverRoomGroup).SendAsync("DeleteMessage", messageId);

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            
            string serverRoomGroup = ConnectedUsers.FirstOrDefault(kvp => kvp.Value.Contains(Context.ConnectionId)).Key;

            ConnectedUsers[serverRoomGroup].Remove(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, serverRoomGroup);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

