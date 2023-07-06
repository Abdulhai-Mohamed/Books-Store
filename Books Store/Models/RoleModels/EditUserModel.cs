using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.RoleModels
{
    public class EditUserModel
    {
        public EditUserModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FavouriteGame { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}
