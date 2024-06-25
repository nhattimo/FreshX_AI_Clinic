using System.ComponentModel.DataAnnotations;

namespace API.Server.Models
{
    public class Roles
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public Employees Employee { get; set; }  // One-to-one relationship
    }
}
