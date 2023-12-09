using System.ComponentModel.DataAnnotations;

namespace Books_Store.Models.UserModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
