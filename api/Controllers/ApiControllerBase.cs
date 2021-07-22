
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermarket.DTO;

namespace Supermarket.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiControllerBase: ControllerBase
    {
        [NonAction] //Set not Tracking http method
        public ObjectResult StatusCodeResult(OperationResult result)
        {
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }
    }
}
