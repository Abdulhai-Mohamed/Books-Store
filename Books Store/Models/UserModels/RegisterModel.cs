using Books_Store.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.UserModels
{
    public class RegisterModel
    {
        [Required,
        EmailAddress]
        //We have decorated the Email property with the[Remote] attribute pointing it to
        //the action method that should be invoked when the email value changes.
        [Remote(action: "IsEmailInUse", controller: "Account")]

        //Use the custom validation attribute
        //this attribute done in server side because it is not generate data-val-* attribute
        //so jquery.validate.unobtrusive.js not see it(its attribute)
        [ValidEmailDomain(allowedDomain: "gmail.com",
        ErrorMessage = "ops, Email domain must be gmail.com for loca accounts, or you can register by your extenal account")]
        public string Email { get; set; }




        [Required,
        DataType(DataType.Password)]
        public string Password { get; set; }




        [Required,
        DataType(DataType.Password),
         Compare("Password", ErrorMessage = "yastaa Password and confirmation password do not match."),
         Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }



        //properties from to map to extended ApplicationUser class
        [Display(Name = "User Favorite Game")]
        public string myFavGame { get; set; }
    }
}
