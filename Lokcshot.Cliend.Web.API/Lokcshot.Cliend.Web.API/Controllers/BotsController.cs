using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Discord.Common.Bots;
using Refit;

namespace Lokcshot.Cliend.Web.API.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class BotsController : ControllerBase
    {

        private readonly IBotsRefit _botsRefit;

        public BotsController(IBotsRefit botsRefit) {
        
            _botsRefit = botsRefit;
        
        }

        [HttpPost("/message")]
        public async Task<ActionResult<BotResponse>> GetMessage([FromBody]BotRequest message)
        {

            try
            {

                var result = await _botsRefit.GetMessage(message);

                return Ok(result);

            }
            catch (ValidationApiException ex)
            {
                return BadRequest(ex.Content);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
