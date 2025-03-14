using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Discord.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lokcshot.Cliend.Web.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ServersController : ControllerBase 
    {

        private readonly IServersRefit _serversRefit;

        public ServersController(IServersRefit serversRefit)
        {
            _serversRefit = serversRefit;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> CreateServer(ServerCreateModel model)
        {

            try
            {

                var result = await _serversRefit.CreateServer(model);

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("Server/{ServerCode}")]
        public async Task<ActionResult<ServerResponseModel>> GetServerByLinq(string ServerCode)
        {

            try
            {

                return Ok(await _serversRefit.FindServerByLinq(ServerCode));

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            
            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("join")]
        public async Task<ActionResult> JoinServer([FromBody] JoinServerModel model)
        {

            try
            {

                await _serversRefit.JoinServer(model);

                return Ok("Success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            
            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpDelete("join")]
        public async Task<ActionResult> ExitServer([FromBody] JoinServerModel model)
        {

            try
            {

                await _serversRefit.ExitServer(model);

                return Ok("Success");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ServerResponseModel>>> GetServersByUserId(Guid userId)
        {

            try
            {

                var result = await _serversRefit.GetServersByUserId(userId);

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            
            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("Users/{code}")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetUsersByServerCode(string code)
        {

            try
            {

                var result = await _serversRefit.GetUsersByServerCode(code);

                return Ok(result);

            } 
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("Channel/{channelCode}")]
        public async Task<ActionResult<ChannelGetModel>> GetChannelByCode(string channelCode)
        {

            try
            {

                var result = await _serversRefit.GetChannelByCode(channelCode);

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("Channel")]
        public async Task<ActionResult> CreateChannel(ChannelCreateModel model)
        {

            try
            {

                await _serversRefit.CreateChannel(model);

                return Ok();

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpDelete("Channel")]
        public async Task<ActionResult> DeleteChannelByCode(ChannelDeleteModel model)
        {

            try
            {

                await _serversRefit.DeleteChannel(model);

                return Ok();

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("Channels/{serverCode}")]
        public async Task<ActionResult<IEnumerable<ChannelGetModel>>> GetChannelsByServerCode(string serverCode)
        {

            try
            {

                var result = await _serversRefit.GetChannelsToServer(serverCode);

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("Messages/{channelCode}")]
        public async Task<ActionResult<IEnumerable<MessageResponseModel>>> GetMessagesToChannel(string channelCode)
        {

            try
            {

                var result = await _serversRefit.GetMessagesToChannel(channelCode);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("Message")]
        public async Task<ActionResult<MessageResponseModel>> PostMessage(MessageRequestModel model)
        {

            try
            {

                var result = await _serversRefit.PostMessage(model);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpDelete("Message")]
        public async Task<ActionResult> DeleteMessage(Guid messageId)
        {

            try
            {

                await _serversRefit.DeleteMessage(messageId);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
