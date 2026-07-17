

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing.Tree;

public class AuthorizationMiddleware
{

    RequestDelegate _next;
    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task Invoke(HttpContext context)
    {


        var userClaims = context.User;

        //now we need to inspect what roles are required on the route so we need to inspect its metadata right just like nestjs.
        var endpoint = context.GetEndpoint();
        var requiredRoles = endpoint?.Metadata.GetMetadata<RolesAttribute>();

        if (requiredRoles?.Roles.Length > 0)
        {

            if (context.User.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedException("user is not authenticated");
            }

            //user will always have a role if they are authenticated
            var userRole = userClaims.FindFirst(ClaimTypes.Role)?.Value!;

            if (userRole == null)
            {
                throw new ForbiddenException("User has no role.");
            }

            if (!requiredRoles.Roles.Any(a => a == userRole))
            {
                throw new ForbiddenException("Role is not allowed");
            }

        }

        await _next(context);

    }
}