using System.ComponentModel.DataAnnotations;

namespace API.Server.DTOs.RolesDTO
{
    public class CreateRolesRequersDto
    {
        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
