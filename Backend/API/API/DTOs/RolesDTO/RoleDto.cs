using System.ComponentModel.DataAnnotations;

namespace API.Server.DTOs.RolesDTO
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }
    }
}
