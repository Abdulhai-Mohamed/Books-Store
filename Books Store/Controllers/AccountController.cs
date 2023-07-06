using Books_Store.Models.SendEmailsModels;
using Books_Store.Models.UserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

//11112345#aA
namespace Books_Store.Controllers
{
    //Identitiy system:

    //UserManager<IdentityUser> 
    //SignInManager<IdentityUser>

    //RoleManager<IdentityRole>
    //SignInManager<IdentityRole>

    //dont forget you can Extend IdentityUser/IdentityRole

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> /*UserManager<IdentityUser> */userManager;
        private readonly SignInManager<ApplicationUser> /*SignInManager<IdentityUser>*/signInManager;
        private readonly ILogger<AccountController> ILogger_AccountController;
        private readonly IEmailService _emailService;
        string EmailMessageBody { get; set; }


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailService emailService
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.ILogger_AccountController = logger;
            _emailService = emailService;


        }
        //CREATE
        #region 1-Create-Register User
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                //1-
                // create new user by  Copy data from RegisterViewModel to our user(ApplicationUser)
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,

                    //Populate City property of ApplicationUser instance which is then passed to the
                    //CreateAsync() method of UserManager class.The data in the ApplicationUser instance is
                    //then saved to the AspNetUsers table by the IdentityDbContext class.
                    FavGame = model.myFavGame

                };

                //2-
                // Store user data in AspNetUsers database table
                IdentityResult identityResult = await userManager.CreateAsync(user, model.Password);


                if (identityResult.Succeeded)
                {
                    //3-
                    //generate confirmation token
                    string EmailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //This method takes one parameter: The user for whom we want to generate the
                    //email confirmation token, also 
                    //You should add, either the default token providers or your own custom token providers that
                    //can generate tokens. Otherwise you would get the following runtime exception:
                    //NotSupportedException: No IUserTwoFactorTokenProvider<TUser> named 'Default' is registered.
                    //so to add default token provider in register ideentity:  .AddDefaultTokenProviders();

                    //4-
                    //generate confirmation Link
                    string confirmationLink = Url.Action
                    ("ConfirmEmail", "Account", new { userId = user.Id, token = EmailConfirmationToken }, Request.Scheme);
                    //The generated confirmation link would look like the following:
                    // https://localhost:44304/Account/ConfirmEmail?userId=987009e3-7f78-445e-8bb8-4400ba886550&token=CfDJ8Hpirs
                    //The last parameter Request.Scheme returns the request protocol such as Http or Https.
                    //This parameter is required to generate the full absolute URL.If this parameter is
                    //not specified, a relative URL like the following will be generated like:
                    // Acco/Account/ConfirmEmail?userId=987009e3-7f78-445e-8bb8-4400ba886550&token=CfDJ8Hpirs

                    ILogger_AccountController.LogWarning(confirmationLink);

                    EmailMessageBody = $"<p>Dear Mr. {user.UserName},</p>" +
                               $"<p>   Thank you for choosing our Application. We are happy to have you as a new user.</p>" +
                               $"<p>Please click on the button below to verify your email address and activate your account:</p>" +
                               $"<p><a href='{confirmationLink}'>Verify Email Address</a></p>" +
                               $"<p>If you have any questions or concerns, please do not hesitate to contact us.</p>" +
                               $"<p>Best regards,<br>Abdul-Hai Mohamed</p>";

                    //await _emailService.SendEmailAsync(model.Email, "Confirm your Email", EmailMessageBody);

                    //5-
                    // If there is already user is already signed in and in this user in Admin role, then it is
                    // the Admin user that is creating a new user. So redirect the Admin user to ListRoles action
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    //6-
                    else//not signed in and not in admin role === normal user
                    {


                        //*here we sign him directly, but as best practice we want confirm email first
                        ////sign -in the user using SignInAsync and redirect to index action of HomeController
                        //await signInManager.SignInAsync(user, isPersistent: false);
                        //return RedirectToAction("index", "home");


                        //redirect him to error view with info to confirm

                        ViewBag.AlertTitle = "Registration successful!";
                        ViewBag.AlertMessage = "Before you can Login, please confirm your " +
                                "email, by clicking on the confirmation link we have emailed you, click it  before 5 minutes to avoid invalid token errors";
                        return View("AlertView");
                        //return RedirectToAction("login", "account");

                    }

                }



                else
                {
                    // If there are any errors, add them to the ModelState object
                    // which will be displayed by the validation summary tag helper
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return View(model);
        }

        #endregion

        #region 2-confirmation Email
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //1-
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            //2-Validate user
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("Centralised 404 error handling");
            }


            //3-Validate the token
            string TokenProvider = userManager.Options.Tokens.EmailConfirmationTokenProvider;
            string TokenPropose = UserManager<ApplicationUser>.ConfirmEmailTokenPurpose;
            //method Returns a flag indicating whether the specified token is valid for the user and purpose
            bool tokenIsValid = await userManager.VerifyUserTokenAsync(user, TokenProvider, TokenPropose, token);
            if (!tokenIsValid)
            {
                ViewBag.ErrorTitle = "Token not valid";
                ViewBag.ErrorMessage = "Please provide Vaild Token";
                return View("Global Exception Handling");
            }

            //4-
            //ConfirmEmailAsync Validates that an email confirmation token matches the specified user
            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);
            //Upon successful email confirmation, this method sets EmailConfirmed column to True in AspNetUsers table.
            if (result.Succeeded)
            {
                ViewBag.AlertTitle = "Confirmation successful!";
                ViewBag.AlertMessage = "Thank you for confirming your email";
                return View("AlertView");
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            ViewBag.ErrorMessage = "Please contact support on abdo@abdo.com";

            return View("Global Exception Handling");
        }
        #endregion

        #region 2-Action that will be used as Remote validation for Register model

        //Remote validation allows a controller action method to be called using client side script.
        //This is very useful when you want to call a server side method without a full page post back.
        //forexample:- Checking if the provided email is already taken by another user can only be done
        //on the server side.but we want validate this directly to the user by client side, for that we use remote action
        //The following IsEmailInUse() controller action method checks if the provided email is in use.

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            //This method should respond to both HTTP GET and POST. This is the reason we specified both the
            //HTTP verbs(Get and Post) using [AcceptVerbs] attribute.
            //ASP.NET Core MVC uses jQuery remote() method which in turn issues an AJAX call to invoke the
            //server side method.The jQuery remote() method expects a JSON response, this is the reason
            //we are returning JSON response from the server-side method(IsEmailInUse)

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"ops, this Email {email} is already in use.");
            }
        }
        #endregion

        #region 4-Login USER

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginModel model = new LoginModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                //GetExternalAuthenticationSchemesAsync() method of SignInManager service, returns the list
                //of all configured external identity providers like (Google, Facebook etc).
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                var user = await userManager.FindByEmailAsync(model.Email);

                if
                (
                    user != null &&
                    !user.EmailConfirmed &&
                    (await userManager.CheckPasswordAsync(user, model.Password))
                )
                {
                    ModelState.AddModelError(string.Empty, "from local login Email not confirmed yet");
                    //This error message is displayed only, if the Email is not confirmed AND the user has
                    //provided correct username and password. 
                    return View(model);
                }




                SignInResult signInResult = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, true);

                //Note
                //1-
                //with RequireConfirmedEmail property is set to true and the email address is
                //not confirmed yet=> PasswordSignInAsync() return NotAllowed as the result,
                //even if we supply the correct username and password.
                //2-
                // The last boolean parameter lockoutOnFailure indicates if the account
                // should be locked on failed logon attempt.
                // On every failed login  attempt AccessFailedCount column value in AspNetUsers table is
                // incremented by 1. When the AccessFailedCount reaches the configured
                // MaxFailedAccessAttempts which in our case is 6, the account will be
                // locked and we have true for IsLockedOut property
                // also LockoutEnd column is populated to the end date which configured in DefaultLockoutTimeSpan
                // also AccessFailedCount column will be reset to 0.
                // After the account is lockedout, ##even if we provide the correct username and password,
                // PasswordSignInAsync() method returns true for IsLockedOut property  and the login
                // will not be allowed for the duration the account is locked, also note that any tries to login
                //in the duration of LockoutEnd => this tries will not count for AccessFailedCount column,
                //because user is basically locked,
                //after we reach time in LockoutEnd column => IsLockedOut return false;

                if (signInResult.Succeeded)
                {


                    //Note that  we do not specify asp - controller = "Account" asp - action = "login" for  this form because 2 reasons:
                    //1 - basically the form submited to the same action that the view get by it
                    //2 - when form submited we do not want set fixed route to go , instaed we want go to the value of ReturnUrl querystring
                    //and this value will be null if we fixed explcitly set form posted to Account/ login
                    if (!string.IsNullOrEmpty(returnUrl))
                    {


                        //Your application is vulnerable to open redirect attacks if the following two
                        //conditions are true:1-Your application redirects to a URL that's specified via the
                        //request such as the querystring or form data,2-The redirection is performed without
                        //checking if the URL is a local URL
                        //To prevent open redirect attacks, check if the provided URL is a local URL or you
                        //are only redirecting to known trusted websites.

                        //return Redirect(returnUrl);


                        //ASP.NET Core has built-in support for local redirection. Simply use the
                        //LocalRedirect() method.If a non - local URL is specified an exception is thrown:
                        return LocalRedirect(returnUrl);
                        //==
                        //if (Url.IsLocalUrl(returnUrl))
                        //{
                        //    return Redirect(returnUrl);
                        //}

                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                // If account is lockedout send the use to AccountLocked view
                else if (signInResult.IsLockedOut)
                {
                    return View("AccountLocked");
                    //After the account is locked, there are 2 options.Wait for the account lockout time
                    //to expire which in our case is 1 minutes and then try again or request password reset
                    //if you have forgotten the password.
                    //Upon successful password reset, set the account lockout end date to current UTC date time
                    //, so the user can login with the new password. Use SetLockoutEndDateAsync() method of the
                    //UserManager service for this.


                }

                else
                {
                    ModelState.AddModelError(string.Empty, "ops Invalid Login Attempt");

                }
            }

            return View(model);
        }


        /* 
         
         If you're experiencing sign-in failures in ASP.NET Core after changing a user's username, it could be because the user's security stamp has not been updated.

When a user's security stamp is updated, any existing authentication tokens become invalid. By default, ASP.NET Core updates the security stamp whenever a user's password is changed, but not when other fields like the username are changed.

To update the security stamp when changing the username, you can use the UserManager.UpdateSecurityStampAsync method. This method updates the user's security stamp, which will cause any existing authentication tokens to become invalid.

Here's an example of how you could update the security stamp when changing a user's username:

javascript
Copy code
var user = await userManager.FindByEmailAsync(model.Email);

if (user != null)
{
    user.UserName = model.NewUsername;
    await userManager.UpdateAsync(user);

    // Update the security stamp to invalidate any existing authentication tokens
    await userManager.UpdateSecurityStampAsync(user);

    var signInResult = await signInManager.PasswordSignInAsync(
        user.Email, model.Password, model.RememberMe, false);

    if (signInResult.Succeeded)
    {
        // Redirect to the appropriate page
    }
    else
    {
        // Handle sign-in failure
    }
}
In this example, we're using the UserManager.UpdateSecurityStampAsync method to update the user's security stamp after changing the username. This should ensure that any existing authentication tokens become invalid, and that the user can sign in with their new username.
         */

        #endregion


        #region 5-External Login



        //Steps To use an external identity provider like Google:

        //1-register our application with Google website, Upon successful registration we will be provided
        //with Client Id and Client Secret which we need to use Google authentication.
        //2-Registering our application with Google and integrating Google authentication in our
        //asp.net core application by using iServiceCollection.AddAuthentication().AddGoogle(
        //3-Use the ExternalLogin action in AccountController to redirect the user to the
        //external login provider sign-in page
        //4-for google website setting=> Authorized redirect URIs -
        //This is the path in our
        //application that users are redirected to after they are authenticated by Google.
        //The default path in asp.net core is signin-google. So the complete redirect URI is
        //Application Root URI/signin-google. If we do not like this default path signin-google
        //we can change it by GoogleOptions.CallbackPath="";.

        //5-create a action to handle call back from the external provider
        //this actio job is to use the returned info to log the user in our app

        [AllowAnonymous]
        [HttpPost]
        //in Login.cshtml Since the button name is set to provider, asp.net core model binding
        //maps the provider name which is Google to provider parameter on the ExternalLogin action.
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {


            //1- 
            //set the redirectUrl(controller/action), so here we pass also querystring returnUrl
            //so the full redirect url=>
            //Root URI/Account/ExternalLoginCallback
            string redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });




            //2-
            //Configure External Authentication Properties:
            AuthenticationProperties authenticationProperties = signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);



            //3-this liene is the line that makes ExternalLogin action in AccountController to
            //redirect the user to the external login provider sign-in page
            return new ChallengeResult(provider, authenticationProperties);
        }


        [AllowAnonymous]
        public async Task<IActionResult>
        ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            //1-
            //set the root URL as the returnurl if the coming returnurl is null
            returnUrl = returnUrl ?? Url.Content("~/");


            //2-
            //populate a login model to pass it to the login view if the external signin failed because provider remote errors
            LoginModel loginModel = new LoginModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                //GetExternalAuthenticationSchemesAsync() method of SignInManager service, returns the list
                //of all configured external identity providers like (Google, Facebook etc).
            };

            //3-
            //check if there remote Errors
            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginModel);
            }

            //4-
            // Get the login information about the user from the external login provider
            ExternalLoginInfo externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();

            //check if there is no info returned by provider for the specified user login atempt
            if (externalLoginInfo == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginModel);
            }

            //5-
            // Get the email claim from external login provider (Google, Facebook etc) 
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            //and check confirmation
            if (email != null)
            {
                // Find the user
                user = await userManager.FindByEmailAsync(email);

                // If email is not confirmed, display login view with validation error
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "from ex login Email not confirmed yet");
                    return View("Login", loginModel);
                }
            }



            //5-
            //to sign in user using ex provider, user must has a login record in AspNetUserLogins table
            //a record consist of 4 columns, 3 of them realted to the properties retuend in ExternalLoginInfo
            //these 3 props are externalLoginInfo.LoginProvider,externalLoginInfo.ProviderKey,externalLoginInfo.ProviderDisplayName
            //the fourth column is the user id, user id column is FK from aspusers table, this mean 
            //to signin the user must has a row contain user id column first in our aspusers table 



            //6-
            //ExternalLoginSignInAsync sign in user after chech his record in AspNetUserLogins table
            SignInResult signInResult = await signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, isPersistent: true, bypassTwoFactor: true);


            //Note
            //with RequireConfirmedEmail property is set to true and the email address is
            //not confirmed yet=> ExternalLoginSignInAsync() return NotAllowed as the result,
            //even if we supply the correct username and password.
            //This error message is displayed only, if the Email is not confirmed AND the user has
            //provided correct username and password. 
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }


            //we perviously check about externalLoginInfo if null, so if reach here this mean the only reason of fail is that
            //the user has no user id so we will create a user row in our aspusers table ,then try to signin
            else
            {
                //7-
                //gather the required info(email) to create a new user



                if (email != null)
                {
                    //another check to see  if we already has user by this emial
                    ApplicationUser NewApplicationUser = await userManager.FindByEmailAsync(email);

                    if (NewApplicationUser == null)
                    {
                        // Create a new user without password if we do not have a user already
                        NewApplicationUser = new ApplicationUser
                        {
                            UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
                        };


                        await userManager.CreateAsync(NewApplicationUser);



                        string EmailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(NewApplicationUser);


                        string confirmationLink = Url.Action
                        ("ConfirmEmail", "Account", new { userId = NewApplicationUser.Id, token = EmailConfirmationToken }, Request.Scheme);


                        ILogger_AccountController.LogWarning(confirmationLink);

                        EmailMessageBody = $"<p>Dear Mr. {NewApplicationUser.UserName},</p>" +
                            $"<p>   Thank you for choosing our Application. We are happy to have you as a new user.</p>" +
                            $"<p>Please click on the button below to verify your email address and activate your account:</p>" +
                            $"<p><a href='{confirmationLink}'>Verify Email Address</a></p>" +
                            $"<p>If you have any questions or concerns, please do not hesitate to contact us.</p>" +
                            $"<p>Best regards,<br>Abdul-Hai Mohamed</p>";

                        await _emailService.SendEmailAsync(NewApplicationUser.Email, "Confirm your Email", EmailMessageBody);




                        ViewBag.AlertTitle = "Registration successful!";
                        ViewBag.AlertMessage = "Before you can Login, please confirm your " +
                                "email, by clicking on the confirmation link we have emailed you, click it  before 5 minutes to avoid invalid token errors";
                        return View("AlertView");

                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table) by pass the user and its externalLoginInfo
                    await userManager.AddLoginAsync(NewApplicationUser, externalLoginInfo);
                    //finaly normal   sign in
                    await signInManager.SignInAsync(NewApplicationUser, isPersistent: true);
                    return LocalRedirect(returnUrl);
                }

                else
                {

                    // If we cannot find the user email from externalLoginInfo  we cannot continue
                    ViewBag.ErrorTitle = $"Email claim not received from: {externalLoginInfo.LoginProvider}";
                    ViewBag.ErrorMessage = "Please contact support on abdo@abdo.com";

                    return View("Global Exception Handling");
                }
            }
        }



        #endregion

        #region 3-Logout USER

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        #endregion




        #region 5-Forgot Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);


                if (user == null)
                {
                    ModelState.AddModelError("", "this account not registerd to our app ");
                    return View();
                }

                var userHasPassword = await userManager.HasPasswordAsync(user);
                if (!userHasPassword)
                {
                    //return RedirectToAction("AddPassword");//he not loged basicaly
                    ModelState.AddModelError("", "this is an exterbal account and donot has password bascailly");
                    return View();
                }




                // If the user is found AND Email is confirmed
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    // Generate the reset password token
                    string token = await userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    string passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);
                    //ex:
                    //https://localhost:44305/Account/ResetPassword?email=pragim@pragimtech.com&token=CfDJ8HpirsZUXNxBvU8n%2...



                    // Log the password reset link
                    ILogger_AccountController.LogWarning(passwordResetLink);

                    EmailMessageBody = $"<p>Dear Mr. {user.UserName},</p>" +
                    $"<p>Please click on the button below to reset your password:</p>" +
                    $"<p><a href='{passwordResetLink}'>Reset Your password</a></p>" +
                    $"<p>If you have any questions or concerns, please do not hesitate to contact us.</p>" +
                    $"<p>Best regards,<br>Abdul-Hai Mohamed</p>";

                    await _emailService.SendEmailAsync(user.Email, "Reset your Password", EmailMessageBody);



                    // Send the user to Forgot Password Confirmation view
                    ViewBag.AlertTitle = "Reset Link Sent successfuly!";
                    ViewBag.AlertMessage = "please confirm your " +
                            "reset password, by clicking on the rest link we have emailed you, click it  before 5 minutes to avoid invalid token errors";
                    return View("AlertView");

                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                ViewBag.AlertTitle = "Reset Link Sent successfuly!";
                ViewBag.AlertMessage = "  If you have an account with us, we have sent an email " +
                    "with the instructions to reset your password, click it  before 5 minutes to avoid invalid token errors";
                return View("AlertView");
            }

            return View(model);
        }

        #endregion


        #region 5-Reset Password


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            //1-
            //To be able to reset the user password we need the following
            //Email, 
            //Password reset token, 
            //New Password and
            //Confirm Password

            //2-
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset Link");
            }


            //3-Validate the token
            var user = await userManager.FindByEmailAsync(email);
            string TokenProvider = userManager.Options.Tokens.PasswordResetTokenProvider;
            string TokenPropose = UserManager<ApplicationUser>.ResetPasswordTokenPurpose;

            //method Returns a flag indicating whether the specified token is valid for the user and purpose
            bool tokenIsValid = await userManager.VerifyUserTokenAsync(user, TokenProvider, TokenPropose, token);

            if (!tokenIsValid)
            {
                ViewBag.ErrorTitle = "Token not valid";
                ViewBag.ErrorMessage = "Please provide Vaild Token";
                return View("Global Exception Handling");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //1-
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {

                    //2-
                    // reset the user password
                    IdentityResult result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    //Resets the user's password to the specified NewPassword after
                    // validating the given password reset token



                    if (result.Succeeded)
                    {
                        //3-reset value of LockoutEnd column  if user locked

                        //After the account is locked, there are 2 options.Wait for the account lockout time
                        //to expire which in our case is 1 minutes and then try again or request password reset
                        //if you have forgotten the password.
                        //Upon successful password reset, set the account lockout end date to current UTC date time
                        //, so the user can login with the new password. Use SetLockoutEndDateAsync() method of the
                        //UserManager service for this.
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }



                        ViewBag.AlertTitle = "Your password is reset successfuly!";
                        ViewBag.AlertMessage = "Now the new password will be used for login";
                        return View("AlertView");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }


                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                ViewBag.AlertTitle = "Your password is reset successfuly!";
                ViewBag.AlertMessage = "Now the new password will be used for login";
                return View("AlertView");
            }

            // Display validation errors if model state is not valid
            return View(model);
        }
        #endregion

        #region 6-change password
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            //redirect the user to AddPassword() action if the user has singed in using an external login account
            //and tries to change password.
            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await signInManager.RefreshSignInAsync(user);
                ViewBag.AlertTitle = "Your password is successfully changed!";
                ViewBag.AlertMessage = "Now the new password will be used for login";
                return View("AlertView");
            }

            return View(model);
        }
        #endregion

        #region 7-Add Password To Local Account linked To External Login Model
        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);
            //redirect the user to ChangePassword() action if the user has singed in using an external login account
            //and already has local password.
            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordToLocalAccountlinkedToExternalLogin_Model model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                //Adds the password to the specified user only if the user
                // does not already have a password.
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                ViewBag.AlertTitle = "You have successfully set a local password!";
                ViewBag.AlertMessage = "You can now use either your local user account or an external account to login, Use your email as the username";
                return View("AlertView");
            }

            return View(model);
        }

        #endregion

        #region 5-Access Denied [Authorize] 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewBag.AccessDenied = "Access Denied page from default route Account/AccessDenied";
            return View();
        }
        #endregion


    }
}
