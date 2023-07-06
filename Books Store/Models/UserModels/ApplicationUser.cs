using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Books_Store.Models.UserModels
{
    //The built-in IdentityUser class has very limited set of properties like Id, Username, Email, PasswordHash etc
    //What if I want to store additional data about the user like Gender, City, Country etc.
    //The built-in IdentityUser class does not have these properties.
    //To store custom user data like Gender, City, Country etc, extend the IdentityUser class.
    public class ApplicationUser : IdentityUser
    {
        //We have included just 3 custom property ,
        //but you can include as many properties as you want.
        //and once you  Add-Migration Extend_IdentityUser
        //this Generate a new migration to add columns to AspNetUsers table
        // Then generates the required migration code to add columns to AspNetUsers table in DB(after update).
        public string City { get; set; }
        public string Region { get; set; }
        public string FavGame { get; set; }


        static public TimeSpan UserExpireTimeSpan { get; set; }


        

    }
}
