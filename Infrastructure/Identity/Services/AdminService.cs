using Application.Features.Admin.Commands.AssignUserPermissions;
using Application.Features.Admin.Commands.CreateAdminUser;
using Application.Features.Admin.Commands.CreateEmployee;
using Application.Interfaces;
using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Identity.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
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
            int userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result.Failure("User not found");

            var isSuperAdmin = await _userManager.IsInRoleAsync(
                user,
                Roles.SuperAdmin);

            if (!isSuperAdmin)
                return Result.Failure("You are not authorized to create an admin");
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

            var roleResult = await _userManager.AddToRoleAsync(
     appUser,
     Roles.Admin);

            if (!roleResult.Succeeded)
            {
                return Result.Failure(
                    roleResult.Errors.Select(e => e.Description).ToList(),
                    "Failed to assign admin role");
            }

            await _unitOfWork.Permissions
                .AssignPermissionsToUserAsync(appUser.Id, request.PermissionIds);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Admin user created successfully");
        }

        public async Task<Result> CreateEmployee(CreateEmployeeCommand request)
        {
            int userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result.Failure("User not found");

            var isSuperAdmin = await _userManager.IsInRoleAsync(
                user,
                Roles.SuperAdmin);
            var permissions = await _unitOfWork.Permissions
            .GetByIdsAsync(request.PermissionIds);

            if (permissions.Count() != request.PermissionIds.Distinct().Count())
                return Result.Failure("One or more permission IDs are invalid");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                return Result.Failure("Email already exists");
            if (!isSuperAdmin)
            {
                var adminPermissions = _unitOfWork.Permissions
               .GetAll()
               .Where(x => x.UserPermissions.Any(x => x.UserId == userId)).ToList();
                var adminModules = adminPermissions
        .Select(p => p.Module)
        .ToHashSet();
                var unauthorizedPermissions = permissions
        .Where(p => !adminModules.Contains(p.Module))
        .ToList();

                if (unauthorizedPermissions.Any())
                {
                    var modules = string.Join(", ",
                        unauthorizedPermissions.Select(p => p.Module).Distinct());

                    return Result.Failure(
                        $"You don't have access to assign permissions for: {modules}");
                }
            }
                
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
