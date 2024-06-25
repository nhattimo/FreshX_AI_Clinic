using System.ComponentModel.DataAnnotations;

namespace API.Server.DTOs.RolesDTO
{
    public class UpdateRolesRequersDto
    {
        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
