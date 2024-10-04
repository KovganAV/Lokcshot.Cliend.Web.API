using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Lokcshot.Cliend.Web.API.Core.Interfaces
{
    public interface IBlobStorageRefit
    {

        [Multipart]
        [Post("/static")]
        Task<string> AddFile([AliasAs("file")] MultipartFormDataContent content);

    }
}
