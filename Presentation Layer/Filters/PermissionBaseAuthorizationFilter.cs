using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation_Layer.Authorization;

namespace Presentation_Layer.Filters;

public class PermissionBaseAuthorizationFilter : IAuthorizationFilter
{

    public AuthorizeHelper AuthorizeHelper { get; }

    public PermissionBaseAuthorizationFilter(AuthorizeHelper authorizeHelper)
    {
        AuthorizeHelper = authorizeHelper;
    }


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var attribute = (CheckPermissionAttribute)context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute);

        if (attribute == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var userPermissions = AuthorizeHelper.GetUserPermissions();

        if (!userPermissions.Contains(attribute.Permission))
        {
            context.Result = new ForbidResult();
        }

    }
}
