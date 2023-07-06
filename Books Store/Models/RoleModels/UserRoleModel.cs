using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Security.Policy;
using Microsoft.Build.Framework;

namespace Books_Store.Models.RoleModels
{
//AspNetUserRoles identity database table

//Application users are stored in AspNetUsers database table, where as roles are stored in AspNetRoles table.
//UserRoles i.e user to role mapping data is stored in AspNetUserRoles table.
//asp.net core aspnetuserroles table
//There is a Many-to-Many relationship between AspNetUsers and AspNetRoles table.A user can be a member of many roles
//and a role can contain many users as it's members. This User and Role mapping data is stored in AspNetUserRoles table.

//This table has just 2 columns - UserId and RoleId.Both are foreign keys.
//UserId column references Id column in AspNetUsers table and RoleId column references Id column in AspNetRoles table.
    public class UserRoleModel
    {

        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }

        //property is required to determine if the user is selected to be a member of the role.
        public bool IsSelected { get; set; }
    }
}
