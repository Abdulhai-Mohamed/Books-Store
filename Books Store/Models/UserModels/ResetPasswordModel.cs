using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.UserModels
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]

        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword",
            ErrorMessage = "yastaa Password and Confirm Password must match")]
        public string ConfirmNewPassword { get; set; }

        public string Token { get; set; }
    }
}
