using System.ComponentModel.DataAnnotations;

namespace TaniLink_Backend.ViewModels
{
    /*public class UserViewModel
    {
    }*/

    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string? Token { get; set; }
        public string? Id { get; set; }
    }
}
