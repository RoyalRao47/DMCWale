using DMCWale.Data.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DMCWale.Admin.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class PagePermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _claimValue;

    public PagePermissionAuthorizeAttribute(string claimValue)
    {
        _claimValue = claimValue;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        if (_claimValue == PagePermissionConstants.ProfileView)
        {
            return;
        }

        if (user.IsInRole(RoleConstants.Admin) ||
            user.HasClaim(PagePermissionConstants.ClaimType, _claimValue))
        {
            return;
        }

        context.Result = new ForbidResult();
    }
}
