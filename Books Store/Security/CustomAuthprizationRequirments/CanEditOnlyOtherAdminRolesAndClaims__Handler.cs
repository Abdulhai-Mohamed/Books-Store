using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json.Linq;
using NLog.Filters;
using NuGet.Configuration;
using System.Data;
using System.Security.Claims;
using System.Security.Policy;

namespace Books_Store.Security.CustomAuthprizationRequirments
{
    public class CanEditOnlyOtherAdminRolesAndClaims__Handler :
                 AuthorizationHandler<ManageAdminRolesAndClaims__Requirement>
    {

        private readonly IHttpContextAccessor iHttpContextAccessor;
        public CanEditOnlyOtherAdminRolesAndClaims__Handler(IHttpContextAccessor contextAccessor)
        {
            iHttpContextAccessor = contextAccessor;
        }



        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageAdminRolesAndClaims__Requirement requirement)
        {





            //Our requirement is pass and the authorization succeeds If the user is
            //in the Admin role AND has Edit Role claim type with a claim value of true
            //AND the admin not edit himself which mean logged -in admin user Id is NOT EQUAL TO the Id of the Admin user being edited
            string loggedInAdminId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminIdBeingEdited = iHttpContextAccessor.HttpContext.Request.Query["userId"];

            if (
                    context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Role" &&
                    claim.Value == "True") &&
                    adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower()
                )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
