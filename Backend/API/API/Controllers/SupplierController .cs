using API.Server.DTOs.Supplier;
using API.Server.Interfaces;
using API.Server.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace API.Server.Controllers
{
    [ApiController]
    [Route("api/supplier")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierInterface _supplierRepo;

        public SupplierController(ISupplierInterface supplierRepo)
        {
            _supplierRepo = supplierRepo;
        }

        [HttpGet]


        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierRepo.GetAllAsync();
            var supplierDtos = suppliers.Select(s => s.ToSupplierDto());
            return Ok(supplierDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var supplier = await _supplierRepo.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier.ToSupplierDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierRequestDto supplierDto)
        {
            var supplierModel = supplierDto.ToSupplierFromCreateDto();
            await _supplierRepo.CreateAsync(supplierModel);
            return CreatedAtAction(nameof(GetById), new { id = supplierModel.Id }, supplierModel.ToSupplierDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSupplierRequestDto updateDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }

            var supplierModel = await _supplierRepo.UpdateAsync(id, updateDto);

            if (supplierModel == null)
            {
                return NotFound();
            }
            return Ok(supplierModel.ToSupplierDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var supplierModel = await _supplierRepo.DeleteAsync(id);
            if (supplierModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
