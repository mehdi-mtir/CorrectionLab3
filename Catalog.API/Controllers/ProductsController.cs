using Catalog.API.DTOs;
using Catalog.API.Models;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les produits ou filtre par catégorie
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] string? category = null)
    {
        var products = await _repository.GetAllAsync(category);
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    /// <summary>
    /// Récupère un produit par son ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { message = $"Product {id} not found" });
        
        return Ok(MapToDto(product));
    }

    /// <summary>
    /// Recherche des produits par nom
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(new { message = "Search term is required" });
        
        var products = await _repository.SearchByNameAsync(searchTerm);
        var productDtos = products.Select(MapToDto);
        return Ok(productDtos);
    }

    /// <summary>
    /// Vérifie la disponibilité d'un produit
    /// </summary>
    [HttpGet("{id}/availability")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CheckAvailability(int id, [FromQuery] int quantity = 1)
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { message = $"Product {id} not found" });
        
        var available = product.StockQuantity >= quantity;
        
        return Ok(new
        {
            productId = id,
            productName = product.Name,
            requestedQuantity = quantity,
            availableStock = product.StockQuantity,
            isAvailable = available,
            message = available 
                ? $"{quantity} unit(s) available" 
                : $"Only {product.StockQuantity} unit(s) in stock"
        });
    }

    /// <summary>
    /// Crée un nouveau produit
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
    {
        if (await _repository.ExistsByNameAsync(createDto.Name))
        {
            return BadRequest(new { message = $"Un produit avec le nom '{createDto.Name}' existe déjà" });
        }

        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            StockQuantity = createDto.StockQuantity,
            Category = createDto.Category,
            ImageUrl = createDto.ImageUrl
        };

        var created = await _repository.CreateAsync(product);
        var productDto = MapToDto(created);
        
        return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
    }

    /// <summary>
    /// Met à jour un produit existant
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
    {
        if (await _repository.ExistsByNameAsync(updateDto.Name, excludeId: id))
        {
            return BadRequest(new { message = $"Un autre produit avec le nom '{updateDto.Name}' existe déjà" });
        }

        var product = new Product
        {
            Name = updateDto.Name,
            Description = updateDto.Description,
            Price = updateDto.Price,
            StockQuantity = updateDto.StockQuantity,
            Category = updateDto.Category,
            ImageUrl = updateDto.ImageUrl
        };

        var updated = await _repository.UpdateAsync(id, product);
        
        if (updated == null)
            return NotFound(new { message = $"Product {id} not found" });
        
        return Ok(MapToDto(updated));
    }

    /// <summary>
    /// Supprime un produit
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        
        if (!deleted)
            return NotFound(new { message = $"Product {id} not found" });
        
        return NoContent();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate
        };
    }
}
