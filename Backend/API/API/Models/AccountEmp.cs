using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace API.Server.Models
{
    public class AccountEmp : IdentityUser
    {
        public string Id { get; set; }

        [Required, EmailAddress]
        public string UserName { get; set; }

        [Required, MaxLength(250), PasswordPropertyText]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Employees Employee { get; set; }  // One-to-one relationship
    }
}

