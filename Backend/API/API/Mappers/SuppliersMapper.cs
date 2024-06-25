using API.Server.DTOs.Supplier;
using API.Server.Models;

namespace API.Server.Mappers
{
    public static class SupplierMapper
    {
        public static SupplierDto ToSupplierDto(this Supplier supplierModel)
        {
            return new SupplierDto
            {
                Id = supplierModel.Id,
                Name = supplierModel.Name,
                Company = supplierModel.Company
            };
        }

        public static Supplier ToSupplierFromCreateDto(this CreateSupplierRequestDto supplierDto)
        {
            return new Supplier
            {
                Name = supplierDto.Name,
                Company = supplierDto.Company
            };
        }

        public static Supplier ToSupplierFromUpdateDto(this UpdateSupplierRequestDto supplierDto, Supplier supplierModel)
        {
            supplierModel.Name = supplierDto.Name;
            supplierModel.Company = supplierDto.Company;
            return supplierModel;
        }
    }
}
