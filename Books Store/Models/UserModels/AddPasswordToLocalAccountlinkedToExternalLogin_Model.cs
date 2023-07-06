using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.UserModels
{
    public class AddPasswordToLocalAccountlinkedToExternalLogin_Model
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
