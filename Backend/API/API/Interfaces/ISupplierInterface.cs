using API.Server.DTOs.Supplier;
using API.Server.Models;

namespace API.Server.Interfaces
{
    public interface ISupplierInterface
    {
        Task<List<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<Supplier> CreateAsync(Supplier supplierModel);
        Task<Supplier?> UpdateAsync(int id, UpdateSupplierRequestDto supplierDto);
        Task<Supplier?> DeleteAsync(int id);
        Task<bool> SupplierExists(int id);
    }
}