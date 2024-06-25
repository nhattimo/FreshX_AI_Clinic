using API.Server.Data;
using API.Server.DTOs.RolesDTO;
using API.Server.Interfaces;
using API.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Server.Services
{
    public class RolesServices : RolesInterface
    {
        private readonly ApplicationDbContext _context;

        public RolesServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Roles>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Roles?> GetByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(i => i.Id == id);

        }
        public async Task<Roles> CreateAsync(Roles roleModel)
        {
            await _context.Roles.AddAsync(roleModel);
            await _context.SaveChangesAsync();
            return roleModel;
        }

        public Task<bool> RoleExists(int id)
        {
            return _context.Roles.AnyAsync(r => r.Id == id);
        }

        public async Task<Roles?> UpdateAsync(int id, UpdateRolesRequersDto roleDto)
        {
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

            if (existingRole == null)
                return null;

            existingRole.RoleName = roleDto.RoleName;
            existingRole.Description = roleDto.Description;

            await _context.SaveChangesAsync();

            return existingRole;
        }
        public async Task<Roles?> DeleteAsync(int id)
        {
            var roleModel = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (roleModel == null)
            {
                return null;
            }
            _context.Roles.Remove(roleModel);
            await _context.SaveChangesAsync();
            return roleModel;
        }
    }
}
