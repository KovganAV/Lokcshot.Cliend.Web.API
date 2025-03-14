using System.Collections.Concurrent;
//using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Proof_of_Concept.Model;

namespace UserActivity.Hubs;

//[Microsoft.AspNetCore.Authorization.Authorize]
public class VoiceChatHub : Hub
{

    private static ConcurrentDictionary<string, ConcurrentDictionary<string, (HashSet<string> Participants, string Creator)>> serverRooms = 
        new ConcurrentDictionary<string, ConcurrentDictionary<string, (HashSet<string>, string)>>();

    private static ConcurrentDictionary<string, (string ServerId, string RoomName)> userRooms = 
        new ConcurrentDictionary<string, (string, string)>();

    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ConnectedUsers = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

    private static ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>> bannedUsers = 
        new ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>>();

    public async Task JoinChats(string serverId)
    {

        if (!ConnectedUsers.ContainsKey(serverId))
        {

            ConnectedUsers[serverId] = new ConcurrentDictionary<string, string>();

        }

        var user = Context.User;
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;

        var existingConnectionId = ConnectedUsers[serverId].FirstOrDefault(x => x.Value == userId).Key;
        if (existingConnectionId != null)
        {
            // Close the existing connection
            await Groups.RemoveFromGroupAsync(existingConnectionId, serverId);
            ConnectedUsers[serverId].TryRemove(existingConnectionId, out _);
        }

        ConnectedUsers[serverId][connectionId] = userId;
        await Groups.AddToGroupAsync(Context.ConnectionId, serverId);

    }

    public async Task CreateRoom(string serverId, string roomName)
    {

        if (!serverRooms.ContainsKey(serverId))
        {
            serverRooms[serverId] = new ConcurrentDictionary<string, (HashSet<string>, string)>();
        }

        if (!serverRooms[serverId].ContainsKey(roomName))
        {
            serverRooms[serverId][roomName] = (new HashSet<string> { Context.ConnectionId }, Context.ConnectionId);
            userRooms[Context.ConnectionId] = (serverId, roomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, serverId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{serverId}:{roomName}");
            await Clients.Group(serverId).SendAsync("roomCreated", roomName);
            await UpdateParticipantsList(serverId, roomName);
            await GetRooms(serverId);
        } else
        {
            await Clients.Caller.SendAsync("roomCreationFailed", "Room already exists.");
        }

    }

    public async Task JoinRoom(string serverId, string roomName)
    {
        if (bannedUsers.TryGetValue(serverId, out var bannedInServer))
        {
            if (bannedInServer.TryGetValue(Context.ConnectionId, out var banExpiry))
            {
                if (DateTime.UtcNow < banExpiry)
                {
                    await Clients.Caller.SendAsync("joinRoomFailed", "You are banned from this room for 30 minutes.");
                    return;
                }
                else
                {
                    bannedInServer.TryRemove(Context.ConnectionId, out _);
                }
            }
        }
        if (serverRooms.TryGetValue(serverId, out var rooms) && rooms.TryGetValue(roomName, out var roomInfo))
        {
            var (participants, creator) = roomInfo;

            if (participants.Contains(Context.ConnectionId))
            {
                await Clients.Caller.SendAsync("alreadyInRoom", roomName);
                return;
            }
            
            if (userRooms.TryGetValue(Context.ConnectionId, out var currentRoom))
            {
                await LeaveRoom(currentRoom.ServerId, currentRoom.RoomName, false);
            }

            participants.Add(Context.ConnectionId);
            userRooms[Context.ConnectionId] = (serverId, roomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{serverId}:{roomName}");
            await Clients.Group($"{serverId}:{roomName}").SendAsync("userJoined", Context.ConnectionId);
            await UpdateParticipantsList(serverId, roomName);
            await GetRooms(serverId);
            await Clients.Group(serverId).SendAsync("updateRoomList", GetRoomList(serverId));
            await Clients.Caller.SendAsync("joinedRoom", roomName);

        }
        else
        {
            await Clients.Caller.SendAsync("joinRoomFailed", "Room does not exist.");
        }
    }

    public async Task LeaveRoom(string serverId, string roomName, bool isCreator)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms) && rooms.TryGetValue(roomName, out var roomInfo))
        {
            var (participants, creator) = roomInfo;
            participants.Remove(Context.ConnectionId);
            userRooms.TryRemove(Context.ConnectionId, out _);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{serverId}:{roomName}");
            await Clients.Group($"{serverId}:{roomName}").SendAsync("userLeft", Context.ConnectionId);

            if (isCreator && Context.ConnectionId == creator)
            {
                if (rooms.TryRemove(roomName, out _))
                {
                    await Clients.Group($"{serverId}:{roomName}").SendAsync("forceLeaveRoom");
                    await Clients.All.SendAsync("roomClosed", roomName); 

                    foreach (var participant in participants)
                    {
                        userRooms.TryRemove(participant, out _);
                        await Groups.RemoveFromGroupAsync(participant, $"{serverId}:{roomName}");
                    }
                }
            }
            else if (participants.Count == 0)
            {
                if (rooms.TryRemove(roomName, out _))
                {
                    await Clients.All.SendAsync("roomClosed", roomName);
                }
            }
            else
            {
                await UpdateParticipantsList(serverId, roomName);
            }

            await GetRooms(serverId);
        }
    }

    public async Task SendSignal(string targetId, string signal)
    {
        await Clients.Client(targetId).SendAsync("receiveSignal", Context.ConnectionId, signal);
    }

    public async Task GetRooms(string serverId)
    {
        await Clients.Group(serverId).SendAsync("roomList", GetRoomList(serverId));
    }

    private List<RoomInfo> GetRoomList(string serverId)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms))
        {
            return rooms.Select(kvp => new RoomInfo
            {
                Name = kvp.Key,
                Participants = kvp.Value.Participants.Select((p, index) => $"User {index + 1}").ToList(),
                CreatorId = kvp.Value.Creator
            }).ToList();
        }
        return new List<RoomInfo>();
    }

    public async Task GetRoomParticipants(string serverId, string roomName)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms) && 
            rooms.TryGetValue(roomName, out var roomInfo))
        {
            var (participants, creator) = roomInfo;
            var participantList = participants
                .Select((participantId, index) => new ParticipantInfo
                {
                    Id = participantId,
                    Name = $"User {index + 1}",
                    IsCreator = participantId == creator
                })
                .ToList();

            await Clients.Group($"{serverId}:{roomName}").SendAsync("updateParticipants", roomName, participantList);
        }
        else
        {
            await Clients.Caller.SendAsync("getRoomParticipantsFailed", "Room not found");
        }
    }

    public async Task UpdateParticipantsList(string serverId, string roomName)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms) && rooms.TryGetValue(roomName, out var roomInfo))
        {
            var participantList = roomInfo.Participants.Select((p, index) => $"User {index + 1}").ToList();
            await Clients.Group($"{serverId}:{roomName}").SendAsync("updateParticipants", participantList);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        foreach (var serverId in ConnectedUsers.Keys)
        {
            if (ConnectedUsers[serverId].Keys.Contains(Context.ConnectionId))
            {
                ConnectedUsers[serverId].TryRemove(Context.ConnectionId, out _);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, serverId);
            }
        }

        if (userRooms.TryRemove(Context.ConnectionId, out var room))
        {
            var (serverId, roomName) = room;
            if (serverRooms.TryGetValue(serverId, out var rooms) && 
                rooms.TryGetValue(roomName, out var roomInfo))
            {
                var (participants, creator) = roomInfo;
                bool isCreator = Context.ConnectionId == creator;

                await LeaveRoom(serverId, roomName, isCreator);
            }
        }

        await base.OnDisconnectedAsync(exception);

    }

    public async Task GetParticipants(string serverId, string roomName)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms) && 
            rooms.TryGetValue(roomName, out var roomInfo))
        {
            var (participants, creator) = roomInfo;
            var participantList = participants
                .Select((participantId, index) => new ParticipantInfo
                {
                    Id = participantId,
                    Name = $"User {index + 1}",
                    IsCreator = participantId == creator
                })
                .ToList();

            await Clients.Caller.SendAsync("receiveParticipants", roomName, participantList);
        }
        else
        {
            await Clients.Caller.SendAsync("getParticipantsFailed", "Room not found");
        }
    }
    public async Task KickUser(string serverId, string roomName, string userIdToKick)
    {
        if (serverRooms.TryGetValue(serverId, out var rooms) && 
            rooms.TryGetValue(roomName, out var roomInfo))
        {
            var (participants, creator) = roomInfo;
        
            if (Context.ConnectionId != creator)
            {
                await Clients.Caller.SendAsync("kickFailed", "Only the room creator can kick users.");
                return;
            }

            if (participants.Remove(userIdToKick))
            {
                await Groups.RemoveFromGroupAsync(userIdToKick, $"{serverId}:{roomName}");
                await Clients.Client(userIdToKick).SendAsync("kicked", roomName);
                await Clients.Group($"{serverId}:{roomName}").SendAsync("userKicked", userIdToKick);

                if (!bannedUsers.ContainsKey(serverId))
                {
                    bannedUsers[serverId] = new ConcurrentDictionary<string, DateTime>();
                }
                bannedUsers[serverId][userIdToKick] = DateTime.UtcNow.AddMinutes(30);

                await UpdateParticipantsList(serverId, roomName);
                await GetRooms(serverId);
            }
        }
    }

    private static readonly ConcurrentDictionary<string, List<string>> _chunks = new ConcurrentDictionary<string, List<string>>();

    public async Task SendScreenCaptureChunk(string chunk, string serverId, string roomName, string userId)
    {
        var key = $"{serverId}:{roomName}:{userId}";
        if (!_chunks.ContainsKey(key))
        {
            _chunks[key] = new List<string>();
        }
        Console.WriteLine(chunk);
        if (chunk == "@!@")
        {
            var dataUrl = "";
            foreach (var chnk in _chunks[key])
            {
                dataUrl+= chnk;
            }
            _chunks.TryRemove(key, out _);
            await Clients.Group($"{serverId}:{roomName}").SendAsync("GetScreenCapture", dataUrl, userId);
        }
        else
        {
            _chunks[key].Add(chunk);
        }
    }

}