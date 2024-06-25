using API.Server.DTOs.CategoryDTO;
using API.Server.Models;

namespace API.Server.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryDto ToCategoryDto(this Category categoryModel)
        {
            return new CategoryDto
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name
            };
        }

        public static Category ToCategoryFromCreateDto(this CreateCategoryRequestDto categoryDto)
        {
            return new Category
            {
                Name = categoryDto.Name
            };
        }

        public static Category ToCategoryFromUpdateDto(this UpdateCategoryRequestDto categoryDto, Category categoryModel)
        {
            categoryModel.Name = categoryDto.Name;
            return categoryModel;
        }
    }
}
