using Catalog.API.Models;

namespace Catalog.API.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? category = null);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}
