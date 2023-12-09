using Books_Store.Models.AuthorModels;
using Books_Store.Models.EF_Core_DB_Models;
using Books_Store.Models.SendEmailsModels;
using Books_Store.Models.UserModels;
using Books_Store.Security.CustomAuthprizationRequirments;
using Books_Store.Security.CustomDataProtectorTokenProvider;
using Books_Store.Security.EncryptionAndDecryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;


//why  i need to define NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
//however i directly can 
//NLog.ILogger logger = LogManager.GetCurrentClassLogger();

//You can certainly use LogManager.GetCurrentClassLogger() to create a logger object without configuring NLog explicitly. However,
//in that case, NLog will use its default configuration, which might not be suitable for your application's needs.

//Configuring NLog using LogManager.Setup().LoadConfigurationFromAppSettings() allows you to customize NLog's behavior and specify
//where and how log messages should be written, among other things. For example, you can configure NLog to write log messages to a
//specific file or database, specify the log message format, configure log levels, and more.

//Additionally, LogManager.Setup().LoadConfigurationFromAppSettings() allows you to load NLog's configuration from your application's
//configuration file (e.g., appsettings.json), which can make it easier to manage and update your application's logging configuration.

//In summary, while it's possible to use LogManager.GetCurrentClassLogger() without explicit configuration, doing so might limit your
//ability to customize and manage your application's logging behavior. On the other hand, configuring NLog explicitly using
//LogManager.Setup().LoadConfigurationFromAppSettings() gives you greater control over your application's logging and allows you
//to tailor it to your specific needs.
Logger logger = NLog.LogManager
    .Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();    //.GetCurrentClassLogger(): This line creates a logger object for the current class.


//although we use ConfigureExceptionHandling middleware to globally handle any error =>
//we also put all the program code inside try and catch, wow globalHandl X2
try
{
    #region Create importantt Instances 

    //create webApplicationBuilder instance
    WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);



    //create serviceProvider instance
    ServiceProvider serviceProvider = webApplicationBuilder.Services.BuildServiceProvider();

    //create IConfiguration instance
    IConfiguration iConfiguration = serviceProvider.GetRequiredService<IConfiguration>();

    //create IWebHostEnvironment instance
    IWebHostEnvironment iWebHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

    //create IServiceCollection instance
    IServiceCollection iServiceCollection = webApplicationBuilder.Services;

    #endregion



    #region Register Services
    //Boss here is => IServiceCollection

    #region 1-Register MVC  service  and  Apply global filtter for Authorize attribute globally  on all controllers and their actions

    //iServiceCollection.AddMvcCore();
    //iServiceCollection.AddControllers();
    IMvcBuilder IMvcBuilder = iServiceCollection.AddMvc(MvcOptions =>
    {
        AuthorizationPolicy authorizationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();


        MvcOptions.Filters.Add(new AuthorizeFilter(authorizationPolicy));



    });

    #endregion

    #region 2-Register Authentication and external logins and persistent sign-in CookieAuthenticationOptions


    AuthenticationBuilder AuthenticationBuilder = iServiceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)


        //To use an external identity provider like Google, we have to first register our application with Google.
        //Upon successful registration we will be provided with Client Id and Client Secret which we need to use Google authentication.
        .AddGoogle(GoogleOptions =>
        {
            GoogleOptions.ClientId = "442523082112-00m69783sed0s8oof5dlsa34isqu4tlq.apps.googleusercontent.com";
            GoogleOptions.ClientSecret = "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";
            //GoogleOptions.CallbackPath="/"; //change default path root/signin-google
        })


        .AddFacebook(FacebookOptions =>
        {
            FacebookOptions.AppId = "221543337502090";
            FacebookOptions.AppSecret = "7d47ed95838729fec8dd5c60abc0a2f8";
            //FacebookOptions.CallbackPath="/"; //change default path root/signin-facebbok
        });

    iServiceCollection.ConfigureApplicationCookie(CookieAuthenticationOptions =>
    {

        //Session Cookie vs Persistent Cookie

        //1-
        //Upon a successful login, a cookie is issued and this cookie is sent with each request to the server.The server uses this cookie
        //to know that the user is already authenticated and logged-in. This cookie can either be a session cookie or a persistent cookie.


        //2-
        //Session cookie =>
        //This cookie is set to expire when the user closes their browser or after a specified ExpireTimeSpan period of inactivity.
        //When the session cookie expires, the user is redirected to the login page

        //Persistent cookie =>
        //active or not acive, This cookie is set to expire only after a specified amount of time ExpireTimeSpan, but it will persist across browser sessions and even
        //if the user closes their browser window. When the persistent cookie expires, the user is redirected to the login page

        //3-
        // by default CookieAuthenticationOptions.ExpireTimeSpan is 14 day, you can set it as you need
        //Important note:
        //Regardless the cookie is Session Cookie or Persistent Cookie => ExpireTimeSpan apllied for both cookies
        //which mean If the user is #inactive for time longer than the ExpireTimeSpan  =>
        //the cookie(Session or Persistent) will expire and the user will be signed out, and in the next request he redirected
        //to login page because he became un authroized

        //in other words 
        //ExpireTimeSpan applied for Session cookie in the case of not active just,
        //however, ExpireTimeSpan applied for Persistent cookie in the both cases active or not active ,

        //also clow browser window end cookie for seesion just, however for Persistent it not end the cookie.



        //3-
        //so what detremine that  the cookie will be a session cookie or a persistent cookie.??
        //basically we have 3 methods that used to sign-in: PasswordSignInAsync and ExternalLoginSignInAsync and SignInAsync

        //each method has bool parameter called isPersistent, so if it false the cookie is session cookie,
        //and if it false the cookie is persistent cookie
        // to be more clear it is the IsPersistent property of AuthenticationProperties object from namespace Microsoft.AspNetCore.Authentication
        // that detremine session cookie or a persistent cookie.
        //so at the end value of IsPersistent property are taken from isPersistent parameter

        //so for PasswordSignInAsync and ExternalLoginSignInAsync they both internaly map value of isPersistent parameter
        //to IsPersistent property by internally calling SignInWithClaimsAsync method namespace Microsoft.AspNetCore.Identity:
        //public virtual Task SignInWithClaimsAsync(TUser user, bool isPersistent, IEnumerable<Claim> additionalClaims)
        //=> SignInWithClaimsAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, additionalClaims);

        //and for SignInAsync method it internaly map value of isPersistent parameter
        //to IsPersistent property by internally calling SignInAsync method namespace Microsoft.AspNetCore.Identity:
        //public virtual Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
        //=> SignInAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, authenticationMethod);

        //and for both 3 methods they calls at end the SignInAsync method from namespace Microsoft.AspNetCore.Authentication

        //4-
        //CookieAuthenticationOptions.SlidingExpiration = true;
        //SlidingExpiration deteremine the behavior of counting the time of ExpireTimeSpan,
        //by enable a sliding expiration to true => as long as the user is active => the expiration time of the cookie is refreshed on
        //each request,and this is teh default behavior as long as the user is active as mentioned.
        //however if it false the time will still counted regardless interactivity of the user


        //6-
        //also to read cookie value you  can do it in c# by 
        //HttpContext.Request.Cookies[".AspNetCore.Application.Identity"]
        //and in js by document.cookie, but the both return value or the name ot the tokecn of the cookei
        //so to return cookie properties like its expirety, in js use cookieStore
        //also js will read cookie only if     CookieAuthenticationOptions.Cookie.HttpOnly = false;







        CookieAuthenticationOptions.ExpireTimeSpan = new TimeSpan(0, 0, 5, 0);
        ApplicationUser.UserExpireTimeSpan = CookieAuthenticationOptions.ExpireTimeSpan;
        //CookieAuthenticationOptions.SlidingExpiration = false;
        CookieAuthenticationOptions.Cookie.HttpOnly = false;
        //CookieAuthenticationOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Set secure policy to always

    });

    // i dont know why CookieAuthen ticationOptions when use AddCookie
    //.AddCookie(CookieAuthenticationOptions =>
    //{
    //    CookieAuthenticationOptions.ExpireTimeSpan = TimeSpan.FromSeconds(100); 
    //    CookieAuthenticationOptions.SlidingExpiration = true;
    //});




    #endregion

    #region 3-Register bulti-in and custom Authorization Policies 

    //1-What is Authorization??

    //Authorization is the process of identifying what the user can and cannot do.
    //For example, if the logged in user is an administrator he may be able to Create,
    //Read, Update and Delete orders, where as a normal user may only view orders
    //but not Create, Update or Delete orders.




    //2- how to apply authorization? 
    //Authorization in ASP.NET Core MVC can controlled through the [Authorize] Attribute



    //2.1-Simple [Authorize] Attribute =>
    //2.1.1-[Authorize] Attribute redirects the user to the login URL with ReturnUrl query string parameter.
    //The URL that we were trying to access will be the value of the ReturnUrl query string parameter.
    //I was trying to Create a New author by navigating to /home/create without first signing in.
    //2.1.2-after login if the user authorized to go to url that exist in ReturnUrl query string, he will go to it
    //if not aspcore redirect him to Account/AccessDenied
    //also you can change this path as shown in next region
    //2.1.3-EX:
    //Since I do not have access to /home/create until I login, ASP.NET core redirected to the
    //login URL which is /Account/Login with the query string parameter ReturnUrl=/home/create
    //Lolhttp://localhost:4901/Account/Login?ReturnUrl=%2Fhome%2Fcreate
    //The characters %2F are the encoded characters for a forward slash (/).
    //To decode these characters in the URL, you may use the following website: https://meyerweb.com/eric/tools/dencoder/



    //2.2-Role Based Access Control (RBAC) or Role Based Authorization. =>
    //in addition to process of [Authorize] Attribute, the Role-based authorization checks Only those users
    //who are members of sepecific Role
    //Ex:
    //Only members of the Administrator role can access the actions in the AdministrationController
    //[Authorize(Roles = "Administrator")]
    //public class AdministrationController : Controller
    //{
    //}
    //Multiple roles can be specified by separating them with a comma:
    //[Authorize(Roles = "Administrator,User")]
    //The actions in this controllerare accessible only to those users who are members of ####either#### Administrator or User role.

    //Multiple Instances of Authorize Attribute:
    //[Authorize(Roles = "Administrator")]
    //[Authorize(Roles = "User")]
    //public class AdministrationController : Controller
    //{
    //}
    //To be able to access the actions in this controller, users have to be members
    //of ####both#### - the Administrator role and the User role.


    //full EX:
    //Members of the Administrator role or the User role can access the controller and the ABC action,
    //but only members of the Administrator role can access the XYZ action.
    //The action Anyone() can be accessed by anyone including the anonymous users as
    //it is decorated with AllowAnonymous attribute.

    //[Authorize(Roles = "Administrator, User")]
    //public class AdministrationController : Controller
    //{
    //    public ActionResult ABC()
    //    {
    //    }

    //    [Authorize(Roles = "Administrator")]
    //    public ActionResult XYZ()
    //    {
    //    }

    //    [AllowAnonymous]
    //    public ActionResult Anyone()
    //    {
    //    }
    //}
    //The Authorize attribute on the AdministrationController protects from the unauthorised access.
    //If the logged-in user is not in Administrator role, asp.net core automatically redirects the user to /Account/AccessDenied.


    //2.3-Claims Based Access Control (CBAC) OR Claims Based Authorization =>
    //in addition to process of [Authorize] Attribute, the Claim-based authorization checks Only those users
    //who are members of sepecific claim
    //2.3.1-create claim policy then use it on controlelr or action
    //EX:
    //[HttpPost]
    //[Authorize(Policy = "DeleteRoleOrUserPolicy")]
    //public async Task<IActionResult> DeleteRole(string id)
    //{
    //    // Delete Role
    //}

    /////////////////////////In ASP.NET Core, a role is just a claim with type Role. 




    //3-What is AllowAnonymous?
    //3.1-[AllowAnonymous] attribute allows anonymous access
    //3.2-Please note: If you apply [AllowAnonymous] attribute at the controller level,any[Authorize] attribute on
    //the same controller actions is ignored.However If you apply [Authorize]  attribute at the controller level,
    //any[AllowAnonymous] attribute on the same controller actions is not ignored.

    //3.3-If you do not have[AllowAnonymous] attribute on the Login actions of the
    //AccountController you will get the following error because the application is stuck in
    //an infinite loop.
    //3.4-Error:
    //HTTP Error 404.15 - Not Found
    //The request filtering module is configured to deny a request where the query string is too long.
    //Most likely causes:
    //Request filtering is configured on the Web server to deny the request because the query string is too long.
    //3.5-Error Explaination:
    //You try to access / Account / login
    //Since the[Authorize] attribute is applied globally, you cannot access the URL / Account / login
    //To login you have to go to / Account / login
    //So the application is stuck in this infinite loop and every time we are redirected, the query string? ReturnUrl =/ Account / Login is appended to the URL
    //This is the reason we get the error -Web server denied the request because the query string is too long.
    //To fix this error, decorate Login() actions in the AccountController with[AllowAnonymous] attribute.

    //the  final URL that cause teh error: Lolhttp://localhost:13511/Account/Login?ReturnUrl=%2FAccount%2FLogin%3FReturnUrl%3D%252FAccount%252FLogin%253FReturnUrl%253D%25252FAccount%25252FLogin%25253FReturnUrl%25253D%2525252FAccount%2525252FLogin%2525253FReturnUrl%2525253D%252525252FAccount%252525252FLogin%252525253FReturnUrl%252525253D%25252525252FAccount%25252525252FLogin%25252525253FReturnUrl%25252525253D%2525252525252FAccount%2525252525252FLogin%2525252525253FReturnUrl%2525252525253D%252525252525252FAccount%252525252525252FLogin%252525252525253FReturnUrl%252525252525253D%25252525252525252FAccount%25252525252525252FLogin%25252525252525253FReturnUrl%25252525252525253D%2525252525252525252FAccount%2525252525252525252FLogin%2525252525252525253FReturnUrl%2525252525252525253D%252525252525252525252FAccount%252525252525252525252FLogin%252525252525252525253FReturnUrl%252525252525252525253D%25252525252525252525252FAccount%25252525252525252525252FLogin%25252525252525252525253FReturnUrl%25252525252525252525253D%2525252525252525252525252FAccount%2525252525252525252525252FLogin%2525252525252525252525253FReturnUrl%2525252525252525252525253D%252525252525252525252525252FAccount%252525252525252525252525252FLogin%252525252525252525252525253FReturnUrl%252525252525252525252525253D%25252525252525252525252525252FAccount%25252525252525252525252525252FLogin%25252525252525252525252525253FReturnUrl%25252525252525252525252525253D%2525252525252525252525252525252FAccount%2525252525252525252525252525252FLogin%2525252525252525252525252525253FReturnUrl%2525252525252525252525252525253D%252525252525252525252525252525252FAccount%252525252525252525252525252525252FLogin%252525252525252525252525252525253FReturnUrl%252525252525252525252525252525253D%25252525252525252525252525252525252FAccount%25252525252525252525252525252525252FLogin%25252525252525252525252525252525253FReturnUrl%25252525252525252525252525252525253D%2525252525252525252525252525252525252FAccount%2525252525252525252525252525252525252FLogin%2525252525252525252525252525252525253FReturnUrl%2525252525252525252525252525252525253D%252525252525252525252525252525252525252FAccount%252525252525252525252525252525252525252FLogin%252525252525252525252525252525252525253FReturnUrl%252525252525252525252525252525252525253D%25252525252525252525252525252525252525252F


    //claim based autorization
    iServiceCollection.AddAuthorization(AuthorizationOptions =>
    {


        //Use AddPolicy() method to create the policy
        //The first parameter is the name of the policy and the second parameter is the policy itself
        //To satisfy this policy requirements, the logged -in user must pass ALL Requirements [ Claims/Roles/assertion ]


        //1-bulti-in Requirements

        AuthorizationOptions.AddPolicy(
            "DeleteRoleOrUserPolicy",
            AuthorizationPolicyBuilder => AuthorizationPolicyBuilder.RequireClaim("Delete Role", "True"));
        //ClaimType comparison is case in-sensitive where as ClaimValue comparison is case sensitive.



        AuthorizationOptions.AddPolicy("CountryPolicy", policy => policy.RequireClaim("Country", "USA", "India", "UK"));
        //To satisfy the following policy the loggedin user must have Country claim with a value of USA, India, or UK



        //Since, a role is also a claim of type role, we can also use a role with the new policy syntax:
        AuthorizationOptions.AddPolicy("AdminTestPolicy", policy => policy.RequireRole("Admin", "User", "Manager"));


        //Create custom authorization policy using RequireAssertion 

        AuthorizationOptions.AddPolicy("AdminPolicy", policy =>
        policy.RequireAssertion(context =>

            context.User.IsInRole("Admin") &&
            context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
            context.User.IsInRole("Super Admin")
        ));
        AuthorizationOptions.AddPolicy("Admin2Policy", policy =>
                policy.RequireAssertion(context =>

                AuthorizeAccess(context)));

        bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                    context.User.IsInRole("Super Admin");
        }


        //2-custom authorization Requirements
        //this policy pass if any of its 2 handlers sucsced ( admin and can edit or super admin)
        AuthorizationOptions.AddPolicy("PolicyWithCustom_AUTH_Requirements", policy =>
               policy.AddRequirements(new ManageAdminRolesAndClaims__Requirement()));




    });

    //Register Http Context Accessor
    iServiceCollection.AddHttpContextAccessor();

    // Register the first handler
    iServiceCollection.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaims__Handler>();
    // Register the second handler
    iServiceCollection.AddSingleton<IAuthorizationHandler, SuperAdmin__Handler>();

    #endregion


    #region 2-Change AccessDenied route 

    iServiceCollection.ConfigureApplicationCookie(ConfigureCookieAuthenticationOptions =>
    {
        ConfigureCookieAuthenticationOptions.AccessDeniedPath = new PathString("/Administration/AccessDenied");

    });


    #endregion


    #region 2-Register IAuthorRepository service by different implementations


    // implementation of MockAuthorRepository
    //iServiceCollection.AddSingleton<IAuthorRepository, MockAuthorRepository>();
    //iServiceCollection.AddScoped<IAuthorRepository, MockAuthorRepository>();
    //iServiceCollection.AddTransient<IAuthorRepository, MockAuthorRepository>();

    // implementation of SQLAuthorRepository
    // Adds a Scoped service of the type specified in IAuthorRepository with an
    // implementation type specified in SQLAuthorRepository to the specified iServiceCollection.
    iServiceCollection.AddScoped<IAuthorRepository, SQLAuthorRepository>();


    #endregion


    #region 3-Register  MyDbContext services in DbContext class in DI

    //Register  MyDbContext service in DbContext class in DI
    iServiceCollection.AddDbContextPool<MyDbContext>(
                dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(   //use the sql server as the EF DB provider
<<<<<<< HEAD
                                                        //for dev
                                                        iConfiguration.GetConnectionString("AuthorDBConnection") //get the connection string from json fles by usng iConfiguration instance
                                                                                                                 //for deploy
                                                                                                                 //"workstation id=BookStoreDBs.mssql.somee.com;packet size=4096;user id=Abdul-HaiM_SQLLogin_1;pwd=fnztzz2f7i;data source=BookStoreDBs.mssql.somee.com;persist security info=False;initial catalog=BookStoreDBs;Encrypt=false;"

=======
                iConfiguration.GetConnectionString("AuthorDBConnection") //get the connection string from json fles by usng iConfiguration instance
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4
                    ));


    #endregion


    #region 4-Register  Identity services using IdentityBuilder

    //AddIdentity() method adds the default identity system configuration for the specified user and role types.
    //IdentityUser/IdentityRole classes is provided by ASP.NET core and contains properties for UserName, PasswordHash, Email etc.
    //This is the class that is used by default by the ASP.NET Core Identity framework to manage registered users/roles
    //of your application,If you want store additional information about the registered users/roles like their
    //Gender, City etc=> Create a custom class that derives from IdentityUser. In this custom class add
    //the additional properties you need and then plug-in this class in AddIdentity() instead of the built-in IdentityUser class 
    IdentityBuilder IdentityBuilder = iServiceCollection.AddIdentity<ApplicationUser, IdentityRole>()


            //We want to store and retrieve User and Role information of the registered
            //users using EntityFrameWork Core from the underlying SQL Server database.
            //We specify this using:
            .AddEntityFrameworkStores<MyDbContext>()


            //Adds the default token providers used to generate tokens for reset passwords, change email
            // and change telephone number operations,etc.. and for two factor authentication token generation.
            .AddDefaultTokenProviders()

            // //Adds our custom token providers and name it 
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("NameOfOurCustomTokenProvider");


    #endregion


    #region 5-Register  Identity Options and Token Providers
    //1-
    //IdentityOptions
    iServiceCollection.Configure<IdentityOptions>(identityOptions =>
    {


        //1-User PASSWORD OPTIONS
        identityOptions.Password.RequiredLength = 10;
        identityOptions.Password.RequiredUniqueChars = 3;
        identityOptions.Password.RequireNonAlphanumeric = false;


        //2-ASP USERS CONFIRMATION EMAIL OPTION
        identityOptions.SignIn.RequireConfirmedEmail = true;


        //3-Account lockout
        //Account lockout is locking (i.e disabling) the account after too many failed logon attempts.

        //Let's say we lock the account for 15 minutes after 5 failed logon attempts. After 15 minutes the
        //user will get another 5 attempts to logon. After 5 failed attempts the account will be locked for
        //another 15 minutes. So this means, it will take many years for an attacker to successfully crack the password. 

        //An organisation may also have password change policy, meaning the password must be changed every 1 or 2
        //months. So account lockout policy combined with password change policy makes it extremely difficult
        //for an attacker to brute-force (i.e guess) password and gain access.

        //Specifies the number of failed logon attempts allowed before the account is locked out. The default is 5.
        identityOptions.Lockout.MaxFailedAccessAttempts = 6;

        //Specifies the amount of the time the account should be locked. The default it 5 minutes.
        identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(6);




        //4-Token Provider
        //By default all tokens takes their values from DataProtectionTokenProvider, but here we
        //Using our Custom provider for  specfic tokens(here for email tokens)  instaed of using
        //default DataProtectionTokenProvider
        identityOptions.Tokens.EmailConfirmationTokenProvider = "NameOfOurCustomTokenProvider";
        //all tokens except EmailConfirmationTokenProvider will still their values from DataProtectionTokenProvider

        identityOptions.Tokens.PasswordResetTokenProvider = "NameOfOurCustomTokenProvider";


    });




    //2-
    //Tokens

    // 2.1
    // -Adjust options of our tokens by using the default DataProtectionTokenProviderOptions => for ALL tokens

    iServiceCollection.Configure<DataProtectionTokenProviderOptions>(DataProtectionTokenProviderOptions =>

        //The following code,  sets the life span of all the tokens generated by DataProtectorTokenProvider class to 5 hours.
        //DataProtectionTokenProviderOptions.TokenLifespan = TimeSpan.FromHours(5));
        DataProtectionTokenProviderOptions.TokenLifespan = TimeSpan.FromMinutes(5));
    //If you want to set the lifespan of just a specific type of token, you can do so by creating a custom token provider.
    //To achieve this we have to create a custom DataProtectorTokenProvider and DataProtectionTokenProviderOptions
    //as next


    //2.2-
    // Adjust options of our tokens by using the default DataProtectionTokenProviderOptions => for ALL tokens

    iServiceCollection.Configure<CustomEmailConfirmationTokenProviderOptions>(CustomEmailConfirmationTokenProviderOptionso =>

            //CustomEmailConfirmationTokenProviderOptionso.TokenLifespan = TimeSpan.FromDays(3));
            CustomEmailConfirmationTokenProviderOptionso.TokenLifespan = TimeSpan.FromMinutes(5));
    //by this any tokens use our this custom provider => will has 3days as life span




    //Note:
    //tokens from DataProtectionTokenProvider are not JWT tokens. DataProtectionTokenProvider is used to generate
    //secure, encrypted, and tamper-proof tokens that are used for various purposes such as password reset,
    //email confirmation, etc. These tokens are not meant to be decoded and verified like JWT tokens.
    //Instead, If your token is generated using the DataProtectionTokenProvider, it is not a JWT token and you cannot
    //use TryValidateToken to validate it. Instead, you can use the UserManager's VerifyUserTokenAsync method this
    //method internally calls ValidateAsync(), to catht reason of their faliures , basically the both logging it and 
    //also you 



    #endregion

    #region 6- Register Encryption class
    iServiceCollection.AddSingleton<OurCustomDataProtectionPurposeStrings>();
    #endregion

    #region 7-Register IEmailConfiguration & IEmailService

    //The GetSection method is used to retrieve a configuration section named "EmailSettings"
    //from the appsettings.json configuration file. The configuration section is represented as
    //an object that can be deserialized into an instance of the EmailConfiguration class.
    //The Get method is then used to deserialize the configuration section into an instance
    ////of the EmailConfiguration class,
    //When the Get method is called on a configuration section, the configuration system deserializes the
    //configuration section into an instance of the specified class.
    //Deserialization is the process of converting data in a specific format (such as JSON or XML) into
    //a corresponding object in memory. In this case, the configuration system uses the JSON data
    //from the "EmailSettings" section of the appsettings.json file and deserializes it into an
    //instance of the EmailConfiguration class.The EmailConfiguration class likely has properties that
    //correspond to the keys in the "EmailSettings" configuration section. For example, if the
    //"EmailSettings" configuration section contains a key called "SmtpServer", the EmailConfiguration
    //class might have a property called SmtpServer of type string.
    //During deserialization, the configuration system reads the values of the keys in the
    //"EmailSettings" section and sets the corresponding properties of the EmailConfiguration object to
    //those values. The deserialized EmailConfiguration object is then returned from the Get method and
    //can be used to configure an email service.
    EmailConfiguration EmailConfigurationInstance = iConfiguration.GetSection("EmailSettings").Get<EmailConfiguration>();
    //Adds a singleton service of the type specified in IEmailConfiguration with an
    // instance specified in EmailConfigurationInstance to the  specified iServiceCollection
    iServiceCollection.AddSingleton<IEmailConfiguration>(EmailConfigurationInstance);


    // Adds a transient service of the type specified in IEmailService with an
    // implementation type specified in EmailService to the specified iServiceCollection.
    iServiceCollection.AddTransient<IEmailService, EmailService>();
    #endregion

    #endregion





    #region 1-Configure nlog Middlerware (must configured before build) and the other logging providers

    //1-
    // Remove all the default logging providers
    webApplicationBuilder.Logging.ClearProviders();

    //2-
    //add the configuration specified in the Logging section of the appsettings.json
    webApplicationBuilder.Logging.AddConfiguration(webApplicationBuilder.Configuration.GetSection("Logging"));

    //3-
    //add them again
    webApplicationBuilder.Logging.AddConsole();
    webApplicationBuilder.Logging.AddDebug();
    // Add NLog as the Logging Provider
    //webApplicationBuilder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

    LogManager.ThrowExceptions = true;
    LogManager.ThrowConfigExceptions = true;
    webApplicationBuilder.Logging.AddNLog();




    //log to file by nlog using c# code not nlog.config
    string shortDate = DateTime.Now.ToShortDateString().Replace('/', '-');
    string logFilePath = Path.Combine(iWebHostEnvironment.ContentRootPath, "wwwroot", "MyNLogLogs");
    //< !--< target name = "anyname" xsi: type = "File" fileName = "c:\DemoLogs\nlog-all-${shortdate}.log" /> -->
    NLog.LogManager.Setup().LoadConfiguration(builder =>
    {
        builder.ForLogger().FilterMinLevel(NLog.LogLevel.Warn).WriteToFile(fileName: $"{logFilePath}/My-Nlog-all-{shortDate}.log");
    });
    webApplicationBuilder.Host.UseNLog();







    #endregion

    //create webApplication instance
    WebApplication webApplication = webApplicationBuilder.Build();

    #region Configure Middlewares
    //Boss here is => IApplicationBuilder

    #region 2-Configure Custom Middlerware 
    //webApplication.Use(async (context,next) =>
    //{
    //    await context.Response.WriteAsync("M1:This is not terminated middle ware");
    //    await next();
    //    await context.Response.WriteAsync("\nM1:response");
    //});

    //webApplication.Use(async (context, next) =>
    //{
    //    await context.Response.WriteAsync("\nM2:This is not terminated middle ware");
    //    await next();
    //    await context.Response.WriteAsync("\nM2:response");
    //});
    //not terminated and pass the resposne in outgoing row 
    //webApplication.MapGet("/", context => context.Response.WriteAsync("\nProcess Name "+ Process.GetCurrentProcess().ProcessName + " 999"));





    // Specify foo.html as the default document
    //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
    //defaultFilesOptions.DefaultFileNames.Clear();
    //defaultFilesOptions.DefaultFileNames.Add("foo.html");
    //webApplication.UseDefaultFiles(defaultFilesOptions);
    //webApplication.UseStaticFiles();


    //test devexception page
    //webApplication.Use( (context,next) =>
    //{
    //    throw new Exception("Error Occurred while processing your request");
    //    next();
    //});

    #endregion

    #region 3-Configure Error handling Middlewares
    //
    if (iWebHostEnvironment.IsDevelopment())
    {
        DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions
        {
            SourceCodeLineCount = 1

        };
        //global handlin in devlop
        webApplication.UseDeveloperExceptionPage(developerExceptionPageOptions);

    }
    else
    {
        #region Handle second type of 404 Error (route page not found)
        //1-
        //To handle non-success http status codes such as 404 for example,
        //we could use the following 3 built-in asp.net core middleware components.

        //UseStatusCodePages
        //UseStatusCodePagesWithRedirects
        //UseStatusCodePagesWithReExecute


        //2-
        //webApplication.UseStatusCodePages(); 
        //With UseStatusCodePages Middleware configured, if we navigate to http://localhost/foo/bar, it returns the following simple text response: Status Code: 404; Not Found

        //3-
        //In a production quality application we want to intercept these non-success http status codes and return a custom error view. To achieve this,
        //we can either use UseStatusCodePagesWithRedirects middleware or UseStatusCodePagesWithReExecute middleware.



        //With the following line in place, if there is a 404 error, the user is redirected to / Error / 404. 
        //whew Erorr is the Erorr controller and The placeholder { 0}, in "/Error/{0}" will automatically receive the http status code.

        //webApplication.UseStatusCodePagesWithRedirects("/Error/{0}");
        webApplication.UseStatusCodePagesWithReExecute("/Error/{0}");


        #endregion




        #region Global exception handling for non devlopment environments   
        // Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
        // The request will not be re-executed if the response has already started.

        //app.UseExceptionHandler("/Error");//redirect the request to error controller
        webApplication.UseExceptionHandler("/Error");

        #endregion


    }

    #endregion


    #region 4-Configure serving files  Middlerwares


    //Configure UseFileServer Middlerware 
    //webApplication.UseFileServer(); //if not comment it , it will override the response of mvc


    //Configure UseStaticFiles Middlerware 
    webApplication.UseStaticFiles();
    #endregion

    #region 5-Configure Identity Authentication & Authorization Middlerwares

    //1-
    //Authentication is the process of identifying who the user is. 
    //We want to be able to authenticate users before the request reaches the MVC middleware.
    //So it's important we add authentication middleware before the MVC middleware in the request processing pipeline.
    webApplication.UseAuthentication();



    //2-Authorization registerd with mvc service 
    //but the middleware configured aftter routing middleware

    //webApplication.UseAuthorization();//Warning ASP0001	The call to UseAuthorization should appear between app.UseRouting() and app.UseEndpoints(..) for authorization to be correctly evaluated    Books Store C:\Abdul - Hai Mohamed\cources\SWE\14 - Projects\Projects\Books Store\Books Store\Program.cs    403 Active

    #endregion

    //webApplication.Use(async (context, next) =>
    //{
    //    var cookie = context.Request.Cookies["AspNetCore.Application.Identity"];
    //    if (cookie != null)
    //    {
    //        if (DateTimeOffset.TryParse(cookie, out DateTimeOffset expiration))
    //        {
    //            var expirationDate = expiration.LocalDateTime;
    //            // use expirationDate as needed
    //        }
    //    }
    //    await next();
    //});
    //webApplication.UseMiddleware<CookieExpirationMiddleware>();

    #region 6-Configure Routing Middlerwares 


    //Configure Routing Middlerware 
    webApplication.UseRouting();

    webApplication.UseAuthorization();

    //Configure MVC Middlerware 
    webApplication.UseEndpoints(iEndpointRouteBuilder =>
    {
        //iEndpointRouteBuilder.MapControllerRoute("The default Route", "{controller=Home}/{action=Index}");//http://localhost:13511/home/test6   // http://localhost:13511/home/details/5 => will return erro404 bc no id route parameter defined in the conventional route pattern  
        //iEndpointRouteBuilder.MapControllerRoute("The default Route", "{action=Index}/{controller=Home}");//http://localhost:13511/test6/home
        iEndpointRouteBuilder.MapControllerRoute("The default Route", "{controller=Home}/{action=Index}/{id?}"); //The question mark, makes the id parameter in the URL optional. This means both the following URLs will be mapped to the Details() action method => / Employees / Details / 1    / Employees / Details
    });

    #endregion


    //run the project
    webApplication.Run();
    #endregion

}

catch (Exception ex)
{
    logger.Error(ex);
    throw;
}
finally
{
    // Ensure to shout downon the NLog ( Disposing )
    NLog.LogManager.Shutdown();
}
