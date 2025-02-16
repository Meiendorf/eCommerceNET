using eCommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProduct productInterface) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        var products = await productInterface.GetAllAsync();
        if (!products.Any())
        {
            return NotFound("No products found");
        }

        var (_, list) = ProductConversion.ToDTO(null, await productInterface.GetAllAsync());

        return list!.Any() ? Ok(list) : NotFound("No products found");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
        var product = await productInterface.FindByIdAsync(id);
        if (product is null)
        {
            return NotFound("Product not found");
        }
        var (_product, _) = ProductConversion.ToDTO(product, null);
        return Ok(_product);
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse>> CreateProduct(ProductDTO product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = product.ToEntity();
        
        return await productInterface.CreateAsync(entity);
    }

    [HttpPut]
    public async Task<ActionResult<GenericResponse>> UpdateProduct(ProductDTO product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = product.ToEntity();

        return await productInterface.UpdateAsync(entity);
    }

    [HttpDelete]
    public async Task<ActionResult<GenericResponse>> DeleteProduct(ProductDTO product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = product.ToEntity();

        return await productInterface.DeleteAsync(entity);
    }
    
}