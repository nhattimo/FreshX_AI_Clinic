using API.Server.DTOs.RolesDTO;
using API.Server.Models;

namespace API.Server.Mappers
{
    public static class RolesMapper
    {
        public static RoleDto ToRoleDto(this Role roleModel)
        {
            return new RoleDto
            {
                Id = roleModel.Id,
                RoleName = roleModel.RoleName,
                //Description = roleModel.Description
            };
        }
        public static Role ToRoleFromCreateDto(this CreateRolesRequersDto roleDto)
        {
            return new Role
            {
                RoleName = roleDto.RoleName,
                //Description = roleDto.Description
            };
        }
    }
}
