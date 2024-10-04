using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;

namespace UserActivity.Hubs
{

    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClientHub : Hub
    {

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ConnectedUsers = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private static readonly ConcurrentDictionary<string, DateTime> LastPingTimes = new ConcurrentDictionary<string, DateTime>();

        private static readonly System.Timers.Timer _timer = new System.Timers.Timer(120000);

        public ClientHub() {

            _timer.Elapsed += async (sender, e) => await CheckPingTimes();

            _timer.Start();

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = ConnectedUsers.Values.FirstOrDefault(d => d.ContainsKey(connectionId))?[connectionId];
            if (userId != null)
            {
                var servers = ConnectedUsers.Where(d => d.Value.ContainsKey(connectionId)).Select(d => d.Key).ToList();
                foreach (var serverId in servers)
                {
                    ConnectedUsers[serverId].TryRemove(connectionId, out _);
                    await Clients.Group(serverId).SendAsync("LeftSite", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetOnlineUsers(string serverId)
        {

            var users = ConnectedUsers[serverId].Values.ToList();

            await Clients.Caller.SendAsync("GetOnlineUsers", users);

        }

        public async Task UserJoinSite(List<string> serversId)
        {

            try
            {

                var user = Context.User;
                var userId = Context.UserIdentifier;

                foreach (var serverId in serversId)
                {

                    var connectionId = Context.ConnectionId;
                    if (!ConnectedUsers.ContainsKey(serverId))
                    {
                        ConnectedUsers[serverId] = new ConcurrentDictionary<string, string>();
                    }

                    // Check if user is already connected to the server
                    var existingConnectionId = ConnectedUsers[serverId].FirstOrDefault(x => x.Value == userId).Key;
                    if (existingConnectionId != null)
                    {
                        // Close the existing connection
                        await Groups.RemoveFromGroupAsync(existingConnectionId, serverId);
                        ConnectedUsers[serverId].TryRemove(existingConnectionId, out _);
                    }

                    // Add the user to the server
                    ConnectedUsers[serverId][connectionId] = userId;
                    await Groups.AddToGroupAsync(connectionId, serverId);
                    await Clients.Group(serverId).SendAsync("JoinSite", userId);

                }

            }
            catch (Exception ex) 
            {

                Console.WriteLine(ex.Message);

            }
        }

        public async Task SendPing()
        {
            var userId = Context.UserIdentifier;
            LastPingTimes[userId] = DateTime.UtcNow;
            await Clients.Caller.SendAsync("ReceivePing", LastPingTimes);
        }


        private static async Task CheckPingTimes()
        {
            var now = DateTime.UtcNow;
            var inactiveUsers = LastPingTimes.Where(kvp => (now - kvp.Value).TotalSeconds > 30).Select(kvp => kvp.Key).ToList();

            foreach (var userId in inactiveUsers)
            {
                LastPingTimes.TryRemove(userId, out _);

                var  connectionId = ConnectedUsers.Values.FirstOrDefault(d => d.Values.Contains(userId))?.FirstOrDefault(kvp => kvp.Value == userId).Key;
                
                if (connectionId != null)
                {
                    var servers = ConnectedUsers.Where(d => d.Value.ContainsKey(connectionId)).Select(d => d.Key).ToList();
                    foreach (var serverId in servers)
                    {
                        ConnectedUsers[serverId].TryRemove(connectionId, out _);
                    }
                }

            }
        }

    }
}
