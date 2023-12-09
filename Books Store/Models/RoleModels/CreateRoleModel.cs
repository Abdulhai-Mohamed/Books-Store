using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.RoleModels
{
    public class CreateRoleModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
