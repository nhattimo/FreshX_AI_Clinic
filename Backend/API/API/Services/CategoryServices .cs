using API.Server.Data;
using API.Server.DTOs.CategoryDTO;
using API.Server.Interfaces;
using API.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Server.Services
{
    public class CategoryServices : ICategoryInterface
    {
        private readonly ApplicationDbContext _context;

        public CategoryServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateAsync(Category categoryModel)
        {
            await _context.Categories.AddAsync(categoryModel);
            await _context.SaveChangesAsync();
            return categoryModel;
        }

        public Task<bool> CategoryExists(int id)
        {
            return _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<Category?> UpdateAsync(int id, UpdateCategoryRequestDto categoryDto)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
                return null;

            existingCategory.Name = categoryDto.Name;

            await _context.SaveChangesAsync();

            return existingCategory;
        }

        public async Task<Category?> DeleteAsync(int id)
        {
            var categoryModel = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoryModel == null)
            {
                return null;
            }
            _context.Categories.Remove(categoryModel);
            await _context.SaveChangesAsync();
            return categoryModel;
        }
    }
}
