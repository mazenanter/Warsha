using Application.Features.Auth.Commands.AdminLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ApiControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(
        AdminLoginCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
    }
}
