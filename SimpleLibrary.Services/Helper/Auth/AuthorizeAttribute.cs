using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using IAuthorizationFilter = Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string Roles { set; get; }

    public AuthorizeAttribute(params string[] roles) : base()
    {
        Roles = string.Join(",", roles);
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.Items[ClaimTypes.NameIdentifier];
        var role = context.HttpContext.Items[ClaimTypes.Role];

        var userRole = string.Empty;
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized Member" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        if (role != null)
        {
            userRole = role.ToString();
        }


        if (string.IsNullOrEmpty(Roles) == false)
        {
            if (string.IsNullOrEmpty(userRole))
            {
                context.Result = new JsonResult(new { message = "Unauthorized Role" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }


            if (Roles.Contains(userRole, StringComparison.OrdinalIgnoreCase) == false)
            {
                context.Result = new JsonResult(new { message = "Unauthorized Role" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}