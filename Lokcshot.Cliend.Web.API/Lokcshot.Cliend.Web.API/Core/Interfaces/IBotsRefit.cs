using Discord.AiBots.Models.Request;
using Discord.AiBots.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Refit;



namespace Lokcshot.Cliend.Web.API.Core.Interfaces
{
    public interface IBotsRefit
    {

        [Post("/api/Bots/message")]
        Task<BotResponse> GetMessage([Body] BotRequest request);

    }
}
