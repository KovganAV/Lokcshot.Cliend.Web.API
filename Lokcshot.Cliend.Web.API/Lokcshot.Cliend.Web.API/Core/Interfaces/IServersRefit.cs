using Discord.Channels.Models.Channels.Requests;
using Discord.Channels.Models.Channels.Responses;
using Discord.Channels.Models.Messages.Requests;
using Discord.Channels.Models.Messages.Responses;
using Discord.Channels.Models.Servers.Requests;
using Discord.Channels.Models.Servers.Responses;
using Refit;

namespace Lokcshot.Cliend.Web.API.Core.Interfaces
{
    public interface IServersRefit
    {

        [Post("/api/Servers")]
        Task<string> CreateServer(ServerCreateModel model);

        [Get("/api/Servers/Server/{ServerCode}")]
        Task<ServerResponseModel> FindServerByLinq(string ServerCode);

        [Post("/api/Servers/join")]
        Task JoinServer([Body] JoinServerModel model);

        [Delete("/api/Servers/join")]
        Task ExitServer([Body] JoinServerModel model);

        [Get("/api/Servers/{userId}")]
        Task<IEnumerable<ServerResponseModel>> GetServersByUserId(Guid userId);

        [Get("/api/Servers/Users/{code}")]
        Task<IEnumerable<Guid>> GetUsersByServerCode(string code);


        [Get("/api/Servers/Channels/{serverCode}")]
        Task<IEnumerable<ChannelGetModel>> GetChannelsToServer(string serverCode);

        [Post("/api/Servers/Channel")]
        Task CreateChannel(ChannelCreateModel model);

        [Delete("/api/Servers/Channel")]
        Task DeleteChannel(ChannelDeleteModel model);

        [Get("/api/Servers/Channel/{channelCode}")]
        Task<ChannelGetModel> GetChannelByCode(string channelCode);

        [Get("/api/Servers/Messages/{channelCode}")]
        Task<IEnumerable<MessageResponseModel>> GetMessagesToChannel(string channelCode);

        [Post("/api/Servers/Message")]
        Task<MessageResponseModel> PostMessage(MessageRequestModel model);
        
        [Delete("/api/Servers/Message")]
        Task DeleteMessage(Guid messageId);

    }
}
