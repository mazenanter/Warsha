using Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      PermissionRequirement requirement)
        {
            Console.WriteLine($"Required Permission: {requirement.Permission}");
            foreach (var claim in context.User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }
            if (context.User.IsInRole(Roles.SuperAdmin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var hasPermission = context.User.Claims
                .Any(c => c.Type == "Permission" && c.Value == requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
