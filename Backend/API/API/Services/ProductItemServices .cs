using API.Server.Data;
using API.Server.DTOs.ProductItemDTO;
using API.Server.Interfaces;
using API.Server.Mappers;
using API.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Server.Services
{
    public class ProductItemServices : IProductItemInterface
    {
        private readonly ApplicationDbContext _context;

        public ProductItemServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductItem>> GetAllAsync()
        {
            return await _context.ProductItems.Include(p => p.Category).Include(p => p.Supplier).ToListAsync();
        }

        public async Task<ProductItem?> GetByIdAsync(int id)
        {
            return await _context.ProductItems.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductItem> CreateAsync(ProductItem productItemModel)
        {
            await _context.ProductItems.AddAsync(productItemModel);
            await _context.SaveChangesAsync();
            return productItemModel;
        }

        public Task<bool> ProductItemExists(int id)
        {
            return _context.ProductItems.AnyAsync(p => p.Id == id);
        }

        public async Task<ProductItem?> UpdateAsync(int id, UpdateProductItemRequestDto productItemDto)
        {
            var existingProductItem = await _context.ProductItems.FirstOrDefaultAsync(p => p.Id == id);

            if (existingProductItem == null)
                return null;

            existingProductItem = productItemDto.ToProductItemFromUpdateDto(existingProductItem);

            await _context.SaveChangesAsync();

            return existingProductItem;
        }

        public async Task<ProductItem?> DeleteAsync(int id)
        {
            var productItemModel = await _context.ProductItems.FirstOrDefaultAsync(p => p.Id == id);
            if (productItemModel == null)
            {
                return null;
            }
            _context.ProductItems.Remove(productItemModel);
            await _context.SaveChangesAsync();
            return productItemModel;
        }
    }
}
