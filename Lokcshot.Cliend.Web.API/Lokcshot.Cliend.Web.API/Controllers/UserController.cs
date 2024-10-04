using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Discord.User.Models.Friends.Request;
using Discord.User.Models.Friends.Response;
using Discord.Common.FriendsManagement;
using Microsoft.AspNetCore.Mvc;
using Discord.User.Models.User.Response;
using Discord.User.Models.User.Request;
using Microsoft.AspNetCore.Authorization;
using Refit;

namespace Lokcshot.Cliend.Web.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserRefit _userRefit;
        private readonly IBlobStorageRefit _blobStorageRefit;
        
        public UserController(IUserRefit userRefit, IBlobStorageRefit blobStorageRefit) 
        {
            _userRefit = userRefit;
            _blobStorageRefit = blobStorageRefit;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("addImage")]
        public async Task<ActionResult<string>> GetImageUrl(IFormFile file)
        {

            try
            {

                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                return await _blobStorageRefit.AddFile(content);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("invites/{id}")]
        public async Task<ActionResult<IEnumerable<InviteResponseModel>>> GetAllInvitesById(Guid id)
        {

            try
            {

                var invites = await _userRefit.GetAllInvitesById(id);

                return Ok(invites);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("invites/check")]
        public async Task<ActionResult<bool>> CheckInvite([FromQuery] InviteAddModel model)
        {

            try
            {

                return await _userRefit.CheckIvite(model);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("invites")]
        public async Task<ActionResult> AddNewInvite([FromBody] InviteAddModel addModel)
        {

            try
            {

                await _userRefit.AddNewInvite(addModel);

                return Ok("Success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserEditModel user, Guid id)
        {

            try
            {

                await _userRefit.UpdateUser(user, id);

                return Ok("success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPut("invites")]
        public async Task<ActionResult> SetInviteStatus([FromBody] InviteRequestModel inviteModel)
        {

            try
            {

                await _userRefit.SetInviteStatus(inviteModel);

                return Ok("Success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("friends/{id}")]
        public async Task<ActionResult<IEnumerable<FriendResponseModel>>> GetAllFriendsById(Guid id)
        {

            try
            {

                var friends = await _userRefit.GetAllFriendsById(id);

                return Ok(friends);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("friends/check")]
        public async Task<ActionResult<bool>> CheckFriends([FromQuery]FriendRequestModel friendRequestModel)
        {

            try
            {

                return Ok(await _userRefit.CheckFriends(friendRequestModel));

            }
            catch (Exception ex) 
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("friends")]
        public async Task<ActionResult> AddNewFriends([FromBody] FriendRequestModel friendModel)
        {

            try
            {

                await _userRefit.AddNewFriends(friendModel);

                return Ok("Success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpDelete("friends/{id}")]
        public async Task<ActionResult> DeleteFriends(Guid friendId, Guid id)
        {

            try
            {

                await _userRefit.DeleteFriends(friendId, id);

                return Ok("Success");

            }
            catch (Exception ex) 
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserFullResponseModel>>> SearchUsers([FromQuery] UsersSearchModel searchModel)
        {

            try
            {

                var users = await _userRefit.SearchUser(searchModel);

                return Ok(users);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            
            }
        }

    }
}
