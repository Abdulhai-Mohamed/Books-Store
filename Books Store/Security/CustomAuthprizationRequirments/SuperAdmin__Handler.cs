using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Books_Store.Security.CustomAuthprizationRequirments
{
    public class SuperAdmin__Handler :
                 AuthorizationHandler<ManageAdminRolesAndClaims__Requirement>
    {

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageAdminRolesAndClaims__Requirement requirement)
        {





            //Our requirement is pass and the authorization succeeds If the user is
            //Is InRole the Super Admin role 
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }
            //note that only super admin can edit himself

            return Task.CompletedTask;
        }

    }
}
