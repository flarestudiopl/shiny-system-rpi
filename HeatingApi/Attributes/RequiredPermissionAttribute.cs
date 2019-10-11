using Domain;
using Microsoft.AspNetCore.Authorization;

namespace HeatingApi.Attributes
{
    public class RequiredPermissionAttribute : AuthorizeAttribute
    {
        public RequiredPermissionAttribute(Permission permission) : base(permission.ToString()) { }
    }
}
