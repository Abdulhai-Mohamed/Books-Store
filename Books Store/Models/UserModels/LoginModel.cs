using Microsoft.AspNetCore.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.Composition;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;

namespace Books_Store.Models.UserModels
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }



        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }








        //is the URL the user was trying to access before authentication.We preserve and pass it between requests using ReturnUrl property, so the user can be redirected to that URL upon successful authentication.
        public string ReturnUrl { get; set; }


        //ExternalLogins property stores the list of external logins (like Facebook, Google etc)
        //that are enabled in our application by
        //iServiceCollection.AddAuthentication().add{provider}()
        public IList<AuthenticationScheme> ExternalLogins { get; set; }




    }
}
