using Application.Features.Admin.Commands.AssignUserPermissions;
using Application.Features.Admin.Commands.CreateAdminUser;
using Application.Features.Admin.Commands.CreateEmployee;
using Application.Interfaces;
using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Identity.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AssignUserPermission(AssignUserPermissionsCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId.ToString());
            if (user is null)
                return Result.Failure("User not found");

            var permissions = await _unitOfWork.Permissions
                .GetByIdsAsync(request.PermissionIds);

            if (permissions.Count() != request.PermissionIds.Distinct().Count())
                return Result.Failure("One or more permission IDs are invalid");

            await _unitOfWork.Permissions.RemoveUserPermissionsAsync(user.Id);
            await _unitOfWork.Permissions
                .AssignPermissionsToUserAsync(user.Id, request.PermissionIds);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Permissions updated successfully");
        }

        public async Task<Result> CreateAdminUser(CreateAdminUserCommand request)
        {
            var permissions = await _unitOfWork.Permissions
            .GetByIdsAsync(request.PermissionIds);

            if (permissions.Count() != request.PermissionIds.Distinct().Count())
                return Result.Failure("One or more permission IDs are invalid");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                return Result.Failure("Email already exists");

            var appUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                UserType = UserType.ADMIN,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);
            if (!result.Succeeded)
                return Result.Failure(
                    result.Errors.Select(e => e.Description).ToList(),
                    "User creation failed");

            await _userManager.AddToRoleAsync(appUser, Roles.Admin);

            await _unitOfWork.Permissions
                .AssignPermissionsToUserAsync(appUser.Id, request.PermissionIds);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Admin user created successfully");
        }

        public async Task<Result> CreateEmployee(CreateEmployeeCommand request)
        {
            var permissions = await _unitOfWork.Permissions
            .GetByIdsAsync(request.PermissionIds);

            if (permissions.Count() != request.PermissionIds.Distinct().Count())
                return Result.Failure("One or more permission IDs are invalid");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                return Result.Failure("Email already exists");

            var appUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                UserType = UserType.EMPLOYEE,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);
            if (!result.Succeeded)
                return Result.Failure(
                    result.Errors.Select(e => e.Description).ToList(),
                    "Employee creation failed");

            await _userManager.AddToRoleAsync(appUser, Roles.Employee);

            await _unitOfWork.Permissions
                .AssignPermissionsToUserAsync(appUser.Id, request.PermissionIds);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Employee created successfully");
        }
    }
}
