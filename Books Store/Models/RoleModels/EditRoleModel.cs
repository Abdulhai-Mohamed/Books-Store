using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.RoleModels
{
    public class EditRoleModel
    {
        public EditRoleModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
