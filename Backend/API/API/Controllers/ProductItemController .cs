using API.Server.DTOs.ProductItemDTO;
using API.Server.Interfaces;
using API.Server.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace API.Server.Controllers
{
    [ApiController]
    [Route("api/productitem")]
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemInterface _productItemRepo;

        public ProductItemController(IProductItemInterface productItemRepo)
        {
            _productItemRepo = productItemRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productItems = await _productItemRepo.GetAllAsync();
            var productItemDtos = productItems.Select(p => p.ToProductItemDto());
            return Ok(productItemDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var productItem = await _productItemRepo.GetByIdAsync(id);
            if (productItem == null)
            {
                return NotFound();
            }
            return Ok(productItem.ToProductItemDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductItemRequestDto productItemDto)
        {
            var productItemModel = productItemDto.ToProductItemFromCreateDto();
            await _productItemRepo.CreateAsync(productItemModel);
            return CreatedAtAction(nameof(GetById), new { id = productItemModel.Id }, productItemModel.ToProductItemDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductItemRequestDto updateDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }

            var productItemModel = await _productItemRepo.UpdateAsync(id, updateDto);

            if (productItemModel == null)
            {
                return NotFound();
            }
            return Ok(productItemModel.ToProductItemDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var productItemModel = await _productItemRepo.DeleteAsync(id);
            if (productItemModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
