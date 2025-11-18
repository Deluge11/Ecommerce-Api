using Enums;
using System.Data;
using System.Security.Claims;

namespace Presentation_Layer.Authorization;

public class AuthorizeHelper
{
    public IHttpContextAccessor _httpContextAccessor { get; }
    public HttpContext HttpContext => _httpContextAccessor.HttpContext;


    public AuthorizeHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Permission> GetUserPermissions()
    {
        var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;

        if (claimIdentity == null)
        {
            return new List<Permission>();
        }

        var permissionClaims = claimIdentity.FindAll("permission");

        var permissions = permissionClaims
            .Select(c => Enum.TryParse<Permission>(c.Value, out var perm) ? perm : default)
            .Where(p => p != default)
            .ToList();

        return permissions;
    }

}
