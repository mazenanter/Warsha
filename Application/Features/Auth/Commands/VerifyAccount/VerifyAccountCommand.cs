using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyAccount
{
    public class VerifyAccountCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
