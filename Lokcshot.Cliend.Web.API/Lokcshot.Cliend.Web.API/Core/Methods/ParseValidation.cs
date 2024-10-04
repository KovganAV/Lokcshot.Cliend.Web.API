using Lokcshot.Cliend.Web.API.Core.Classes;
using Newtonsoft.Json;

namespace Lokcshot.Cliend.Web.API.Core.Methods
{
    public class ParseValidation
    {
        public static string ParseValidationError(string content)
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);

            var validationError = string.Join(" ", errorResponse.Errors.SelectMany(e => e.Value));

            return validationError;
        }
    }
}
