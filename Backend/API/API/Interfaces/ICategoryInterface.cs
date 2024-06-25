using API.Server.DTOs.CategoryDTO;
using API.Server.Models;

namespace API.Server.Interfaces
{
    public interface ICategoryInterface
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category categoryModel);
        Task<Category?> UpdateAsync(int id, UpdateCategoryRequestDto categoryDto);
        Task<Category?> DeleteAsync(int id);
        Task<bool> CategoryExists(int id);
    }
}
