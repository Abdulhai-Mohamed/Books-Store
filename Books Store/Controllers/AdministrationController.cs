using Books_Store.Models.RoleModels;
using Books_Store.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Recommendations;
using System.Drawing;
using System.Dynamic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Xml.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using Books_Store.Models.ClamisModels;

namespace Books_Store.Controllers
{
    //Identitiy system:

    //UserManager<IdentityUser> 
    //SignInManager<IdentityUser>

    //RoleManager<IdentityRole>
    //SignInManager<IdentityRole>

    //dont forget you can Extend IdentityUser/IdentityRole



    //The Authorize attribute on the AdministrationController protects from the unauthorised access.
    //If the logged-in user is not in Admin role, asp.net core automatically redirects the user to /Account/AccessDenied.
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        //CREATE
        #region 1-Create Role

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult identityResult = await roleManager.CreateAsync(identityRole);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        #endregion
        //REAAD
        #region 2-list of roles
        [HttpGet]
        public IActionResult ListRoles()
        {
            IEnumerable<IdentityRole> rolesList = roleManager.Roles;
            return View(rolesList);
        }

        #endregion
        //UPDATE
        #region 3-Edit Role
        // Role ID is passed from the URL to the action
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Centralised 404 error handling");
            }
            EditRoleModel model = new EditRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };




            //The InvalidOperationException: There is already an open DataReader associated with this Connection which
            //must be closed first exception can occur in ASP.NET Core when using Entity Framework to access a database,
            //if there is still an open DataReader associated with the same connection while trying to execute another query.

            //In the code provided, the issue could be related to the fact that the userManager.Users property is being
            //used to retrieve a list of users, while at the same time, there may be an open DataReader associated with
            //the same connection, possibly from another query or operation.

            //To resolve this issue, it is recommended to close the DataReader when it is no longer needed,
            //or use a new database connection to perform the operation.Alternatively, you can try to force
            //synchronous execution of the queries by calling the ToList() method on the userManager.Users property
            //to retrieve the users in a collection and iterate over that collection instead of the userManager.Users property.
            // Retrieve all the Users
            foreach (ApplicationUser user in userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
            }


        // This action responds to HttpPost and receives EditRoleModel
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModel model)
        {
            IdentityRole role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("Centralised 404 error handling");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                IdentityResult identityResult = await roleManager.UpdateAsync(role);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        #endregion
        //DELETE
        #region 4-Delete Role
        //Deleting data using a GET request is not recommended.

        //Just imagine what can happen if there is an image tag in a malicious email as shown below.
        //The moment we open the email, the image tries to load and issues a GET request, which would delete
        //the data.//<img src = "http://localhost/Administration/DeleteUser/123" />
        //Also, when search engines index your page, they issue a GET request which would delete the data.
        //In general GET request should be free of any side-effects, meaning it should not change the state
        //on the server. Deletes should always be performed using a POST request.
        //Deleting data using a POST request is the best
        //Delete button type is set to submit
        //It is placed inside the form element and the method attribute is set to post
        //So when the Delete button is clicked a POST request is issued to DeleteUser() action passing it the ID of the user to delete
        //<form method="post" asp-action="DeleteUser" asp-route-id="@user.Id">
        //    <button type = "submit" class="btn btn-danger">Delete</button>
        //</form>

        [HttpPost]
        //CLAIMS AUTH
        [Authorize(Policy = "DeleteRoleOrUserPolicy")]

        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Centralised 404 error handling");
            }
            else
            {
                try
                {

                    IdentityResult result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }

                //if ypu try delete role has users or user that inside a role => 
                //SqlException: The DELETE statement conflicted with the REFERENCE constraint
                //"FK_AspNetUserRoles_AspNetUsers_UserId".The conflict occurred in database "BookStoreDB",
                //table "dbo.AspNetUserRoles", column 'UserId'.

                // If the exception is DbUpdateException, we know we are not able to
                // delete the role as there are users in the role being deleted
                catch (DbUpdateException ex)
                {
                    //Log the exception to a file:
                    logger.LogError($"Exception Occured : {ex}");

                    // Pass the ErrorTitle and ErrorMessage that you want to show to
                    // the user using ViewBag. The Error view retrieves this data
                    // from the ViewBag and displays to the user.
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role or it used in another Database table. If you want to delete this role, please remove the users or  any data related to this role from other tables and then try to delete";
                    return View("Global Exception Handling");
                }

            }
        }

        #endregion





        //REAAD
        #region 6-list users
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }
        #endregion
        //UPDATE
        #region 7-Edit user


        [HttpGet]

        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("Centralised 404 error handling");
            }

            // GetClaimsAsync retunrs the list of user Claims
            IList<Claim> userClaims = await userManager.GetClaimsAsync(applicationUser);

            // GetRolesAsync returns the list of user Roles
            IList<string> userRoles = await userManager.GetRolesAsync(applicationUser);


            EditUserModel model = new EditUserModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                UserName = applicationUser.UserName,
                FavouriteGame = applicationUser.FavGame,
                Claims = userClaims.Select(c => c.Type /*c.Value*/).ToList(), //select #value of claims
                Roles = userRoles
            };

            return View(model);
        }




        [HttpPost]

        public async Task<IActionResult> EditUser(EditUserModel model)
        {

            ApplicationUser applicationUser = await userManager.FindByIdAsync(model.Id);

            if (applicationUser == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("Centralised 404 error handling");
            }
            else
            {
                applicationUser.Email = model.Email;
                applicationUser.UserName = model.UserName;
                applicationUser.FavGame = model.FavouriteGame;

                IdentityResult identityResult = await userManager.UpdateAsync(applicationUser);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        #endregion
        //DELETE
        #region 8-Delete user
        //Deleting data using a GET request is not recommended.

        //Just imagine what can happen if there is an image tag in a malicious email as shown below.
        //The moment we open the email, the image tries to load and issues a GET request, which would delete
        //the data.//<img src = "http://localhost/Administration/DeleteUser/123" />
        //Also, when search engines index your page, they issue a GET request which would delete the data.
        //In general GET request should be free of any side-effects, meaning it should not change the state
        //on the server. Deletes should always be performed using a POST request.
        //Deleting data using a POST request is the best
        //Delete button type is set to submit
        //It is placed inside the form element and the method attribute is set to post
        //So when the Delete button is clicked a POST request is issued to DeleteUser() action passing it the ID of the user to delete
        //<form method="post" asp-action="DeleteUser" asp-route-id="@user.Id">
        //    <button type = "submit" class="btn btn-danger">Delete</button>
        //</form>

        [HttpPost]
        [Authorize(Policy = "DeleteRoleOrUserPolicy")]

        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("Centralised 404 error handling");
            }
            else
            {
                try
                {

                    IdentityResult result = await userManager.DeleteAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListUsers");

                }

                //if ypu try delete role has users or user that inside a role => 
                //SqlException: The DELETE statement conflicted with the REFERENCE constraint
                //"FK_AspNetUserRoles_AspNetUsers_UserId".The conflict occurred in database "BookStoreDB",
                //table "dbo.AspNetUserRoles", column 'UserId'.

                // If the exception is DbUpdateException, we know we are not able to
                // delete the role as there are users in the role being deleted
                catch (DbUpdateException ex)
                {
                    //Log the exception to a file:
                    logger.LogError($"Exception Occured : {ex}");

                    // Pass the ErrorTitle and ErrorMessage that you want to show to
                    // the user using ViewBag. The Error view retrieves this data
                    // from the ViewBag and displays to the user.
                    ViewBag.ErrorTitle = $"{applicationUser.UserName} user is in use";
                    ViewBag.ErrorMessage = $"{applicationUser.UserName} user cannot be deleted as there are roles use this user or it used in another Database table. If you want to delete this user, please remove the roles or  any data related to this user from other tables and then try to delete";
                    return View("Global Exception Handling");
                }
            }
        }

        #endregion


        #region 5-Edit Users in role

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            IdentityRole role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("Centralised 404 error handling");
            }

            List<UserRoleModel> ListUserRoleModel = new List<UserRoleModel>();

            //The InvalidOperationException: There is already an open DataReader associated with this Connection which
            //must be closed first exception can occur in ASP.NET Core when using Entity Framework to access a database,
            //if there is still an open DataReader associated with the same connection while trying to execute another query.

            //In the code provided, the issue could be related to the fact that the userManager.Users property is being
            //used to retrieve a list of users, while at the same time, there may be an open DataReader associated with
            //the same connection, possibly from another query or operation.

            //To resolve this issue, it is recommended to close the DataReader when it is no longer needed,
            //or use a new database connection to perform the operation.Alternatively, you can try to force
            //synchronous execution of the queries by calling the ToList() method on the userManager.Users property
            //to retrieve the users in a collection and iterate over that collection instead of the userManager.Users property.
            // Retrieve all the Users

            foreach (ApplicationUser applicationUser in userManager.Users.ToList())
            {
                UserRoleModel userRoleModel = new UserRoleModel
                {
                    UserId = applicationUser.Id,
                    UserName = applicationUser.UserName
                };

                if (await userManager.IsInRoleAsync(applicationUser, role.Name))
                {
                    userRoleModel.IsSelected = true;
                }
                else
                {
                    userRoleModel.IsSelected = false;
                }

                ListUserRoleModel.Add(userRoleModel);
            }

            return View(ListUserRoleModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleModel> model, string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                //add user to role if it ( checked  +  user not in this role basically)
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(applicationUser, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(applicationUser, role.Name);
                }
                //remove user from role if it (not checked  +  user is in this role basically)
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(applicationUser, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(applicationUser, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        #endregion


        #region 9- Manage User Roles
        [HttpGet]
        [Authorize(Policy = "PolicyWithCustom_AUTH_Requirements")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            ApplicationUser ApplicationUser = await userManager.FindByIdAsync(userId);

            if (ApplicationUser == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Centralised 404 error handling");
            }

            List<ManageUserRolesModel> model = new List<ManageUserRolesModel>();

            foreach (var role in roleManager.Roles.ToList())
            {
                ManageUserRolesModel userRolesViewModel = new ManageUserRolesModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(ApplicationUser, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesModel> model, string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Centralised 404 error handling");
            }
            // Get all the user existing roles and delete them
            var existingRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, existingRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            //add the selected roles in GUI to the user , and this depending on IsSelected prop
            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }
        #endregion

        #region 10- Manage User Claims
        [HttpGet]
        [Authorize(Policy = "PolicyWithCustom_AUTH_Requirements")]

        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ViewBag.userId = userId;
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Centralised 404 error handling");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            IList<Claim> existingListOfUserClaims = await userManager.GetClaimsAsync(user);

            UserClaimsModel model = new UserClaimsModel
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            foreach (Claim claim in MyClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingListOfUserClaims.Any(userCla => userCla.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);

        }



        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsModel model, string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("Centralised 404 error handling");
            }

            // Get all the user existing claims and delete them
            var existingClaims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, existingClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            //add the selected Cliams in GUI to the user , and this depending on IsSelected prop
            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimValue)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }
        #endregion



        #region 11-Access Denied [Authorize] 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewBag.AccessDenied = "Access Denied page from modified route Administration/AccessDenied";
            return View();
        }
        #endregion
    }

}
