using Application.Features.Auth.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.AdminLogin
{
    public record AdminLoginCommand(
     string Email,
     string Password
 ) : IRequest<Result<AuthResult>>;
}
