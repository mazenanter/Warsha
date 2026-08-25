using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyAccount
{
    public class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand, Result>
    {
        private readonly IAuthService _authService;

        public VerifyAccountCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
           var result =await  _authService.ConfirmEmailAsync(request.Email, request.OtpCode);
            return result;
        }
    }
}
