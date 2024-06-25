using API.Server.DTOs.ProductItemDTO;
using API.Server.Models;

namespace API.Server.Mappers
{
    public static class ProductItemMapper
    {
        public static ProductItemDto ToProductItemDto(this ProductItem productItemModel)
        {
            return new ProductItemDto
            {
                Id = productItemModel.Id,
                Name = productItemModel.Name,
                Description = productItemModel.Description,
                Rating = productItemModel.Rating,
                ImageUrl = productItemModel.ImageUrl,
                CategoryId = productItemModel.CategoryId,
                SupplierId = productItemModel.SupplierId,
                UpdatedAt = productItemModel.UpdatedAt,
                DeletedAt = productItemModel.DeletedAt,
                CreatedAt = productItemModel.CreatedAt
            };
        }

        public static ProductItem ToProductItemFromCreateDto(this CreateProductItemRequestDto productItemDto)
        {
            return new ProductItem
            {
                Name = productItemDto.Name,
                Description = productItemDto.Description,
                Rating = productItemDto.Rating,
                ImageUrl = productItemDto.ImageUrl,
                CategoryId = productItemDto.CategoryId,
                SupplierId = productItemDto.SupplierId,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public static ProductItem ToProductItemFromUpdateDto(this UpdateProductItemRequestDto productItemDto, ProductItem productItemModel)
        {
            productItemModel.Name = productItemDto.Name;
            productItemModel.Description = productItemDto.Description;
            productItemModel.Rating = productItemDto.Rating;
            productItemModel.ImageUrl = productItemDto.ImageUrl;
            productItemModel.CategoryId = productItemDto.CategoryId;
            productItemModel.SupplierId = productItemDto.SupplierId;
            productItemModel.UpdatedAt = DateTime.UtcNow;
            return productItemModel;
        }
    }
}
