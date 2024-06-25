using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace API.Server.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Password don't match.")]
        public string? confirmPassword { get; set; }

    }
}