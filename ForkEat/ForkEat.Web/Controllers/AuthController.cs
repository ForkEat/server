using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserRequest request)
        {
            var registerUserResponse = await authenticationService.Register(request);
            return Created("",registerUserResponse);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request)
        {
            try
            {
                var result = await authenticationService.Login(request);
                return result;
            }
            catch (InvalidCredentialsException exception)
            {
                return Unauthorized();
            }

        }
    }
}