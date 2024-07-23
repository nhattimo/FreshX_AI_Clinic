using API.Server.DTOs.RolesDTO;
using API.Server.Models;

namespace API.Server.Interfaces
{
    public interface RolesInterface
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role roleModel);
        Task<Role?> UpdateAsync(int id, UpdateRolesRequersDto roleDto);
        Task<Role?> DeleteAsync(int id);
        Task<bool> RoleExists(int id);
    }
}
