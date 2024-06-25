using API.Server.DTOs.RolesDTO;
using API.Server.Models;

namespace API.Server.Interfaces
{
    public interface RolesInterface
    {
        Task<List<Roles>> GetAllAsync();
        Task<Roles?> GetByIdAsync(int id);
        Task<Roles> CreateAsync(Roles roleModel);
        Task<Roles?> UpdateAsync(int id, UpdateRolesRequersDto roleDto);
        Task<Roles?> DeleteAsync(int id);
        Task<bool> RoleExists(int id);
    }
}
