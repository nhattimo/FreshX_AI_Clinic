using System.ComponentModel.DataAnnotations;

namespace API.Server.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remeber Me")]
        public bool RememBerMe { get; set; }
    }
}