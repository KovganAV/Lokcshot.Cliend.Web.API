using Discord.User.Models.Friends.Request;
using Discord.User.Models.Friends.Response;
using Discord.User.Models.User.Request;
using Discord.User.Models.User.Response;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Lokcshot.Cliend.Web.API.Core.Interfaces
{
    public interface IUserRefit
    {
        [Get("/api/user")]
        Task<IEnumerable<UserFullResponseModel>> GetAllUsers();

        [Post("/api/auth/login")]
        Task<UserFullResponseModel> Login([Body] UserLoginModel loginModel);

        [Post("/api/auth/register")]
        Task<UserFullResponseModel> Register([Body] UserCreateModel createModel);

        [Get("/api/auth/getUser/{id}")]
        Task<UserFullResponseModel> GetUserFullResponseModelById(Guid id);

        [Put("/api/User/edit/{id}")]
        Task UpdateUser([Body] UserEditModel user, Guid id);

        [Get("/api/User/invites/{id}")]
        Task <IEnumerable<InviteResponseModel>> GetAllInvitesById(Guid id);

        [Get("/api/User/invites/check")]
        Task<bool> CheckIvite([Query] InviteAddModel model);

        [Post("/api/User/invites")]
        Task AddNewInvite([Body] InviteAddModel addModel);

        [Put("/api/User/invites")]
        Task SetInviteStatus([Body] InviteRequestModel inviteModel);


        [Get("/api/User/friends/{id}")]
        Task<IEnumerable<FriendResponseModel>> GetAllFriendsById(Guid id);

        [Get("/api/User/friends/check")]
        Task<bool> CheckFriends([Query] FriendRequestModel model);

        [Post("/api/User/friends")]
        Task AddNewFriends([Body] FriendRequestModel friendModel);

        [Delete("/api/User/friends/{id}")]
        Task DeleteFriends(Guid user2Id, Guid id);

        [Get("/api/User/search")]
        Task<IEnumerable<UserFullResponseModel>> SearchUser([Query] UsersSearchModel searchModel);

    }
}
