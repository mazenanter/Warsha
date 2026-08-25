using Application.Features.Auth.Commands.ClientLogin;
using Application.Features.Auth.Commands.CreateClient;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.ResendOtp;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Commands.VerifyAccount;
using Application.Features.Auth.Commands.WorkshopLogin;
using Application.Features.Auth.Commands.WorkshopRegister;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Api.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        [HttpPost("client/create")]
        public async Task<IActionResult> CreateClient(ClientRegisterCommand command)
        {
            var result  = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("client/refresh-token")]
        public async Task<IActionResult> RefreshTokenClient(RefreshTokenCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("client/login")]
        public async Task<IActionResult> LoginClient(ClientLoginCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("client/forgot-password")]
        public async Task<IActionResult> ForgotPasswordClient(ForgotPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("client/reset-password")]
        public async Task<IActionResult> ResetPasswordClient(ResetPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("client/resend-otp")]
        public async Task<IActionResult> ResendOtpClient(ResendOtpCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("client/verify")]
        public async Task<IActionResult> VerifyClient(VerifyAccountCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("client/logout")]
        public async Task<IActionResult> LogoutClient(RevokeTokenCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("workshop/create")]
        public async Task<IActionResult> CreateWorkshop(WorkshopRegisterCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("workshop/login")]
        public async Task<IActionResult> LoginWorkshop(WorkshopLoginCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("workshop/forgot-password")]
        public async Task<IActionResult> ForgotPasswordWorkshop(ForgotPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleGenericResult(result);
        }
        [HttpPost("workshop/reset-password")]
        public async Task<IActionResult> ResetPasswordWorkshop(ResetPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
