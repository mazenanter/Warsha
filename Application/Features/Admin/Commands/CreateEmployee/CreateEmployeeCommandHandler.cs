using Application.Features.Admin.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Admin.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<CreateAdminAndEmployeeDto>>
    {
        private readonly IAdminService _adminService;

        public CreateEmployeeCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<Result<CreateAdminAndEmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await _adminService.CreateEmployee(request);
        }
    }
}
