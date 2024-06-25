using API.Server.DTOs.ProductItemDTO;
using API.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Server.Interfaces
{
    public interface IProductItemInterface
    {
        Task<List<ProductItem>> GetAllAsync();
        Task<ProductItem?> GetByIdAsync(int id);
        Task<ProductItem> CreateAsync(ProductItem productItemModel);
        Task<ProductItem?> UpdateAsync(int id, UpdateProductItemRequestDto productItemDto);
        Task<ProductItem?> DeleteAsync(int id);
        Task<bool> ProductItemExists(int id);
    }
}
