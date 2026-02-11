using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories;

public class SqlProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<SqlProductRepository> _logger;

    public SqlProductRepository(CatalogDbContext context, ILogger<SqlProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(string? category = null)
    {
        _logger.LogInformation("Récupération des produits. Catégorie: {Category}", category ?? "toutes");
        
        var query = _context.Products.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category.ToLower() == category.ToLower());
        }
        
        return await query
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Récupération du produit avec ID: {ProductId}", id);
        
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _logger.LogInformation("Création d'un nouveau produit: {ProductName}", product.Name);
        
        product.CreatedDate = DateTime.UtcNow;
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Produit créé avec succès. ID: {ProductId}", product.Id);
        
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        _logger.LogInformation("Mise à jour du produit ID: {ProductId}", id);
        
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Produit non trouvé pour mise à jour. ID: {ProductId}", id);
            return null;
        }
        
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.Category = product.Category;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.UpdatedDate = DateTime.UtcNow;
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit mis à jour avec succès. ID: {ProductId}", id);
            return existingProduct;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Erreur de concurrence lors de la mise à jour du produit {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Suppression du produit ID: {ProductId}", id);
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Produit non trouvé pour suppression. ID: {ProductId}", id);
            return false;
        }
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Produit supprimé avec succès. ID: {ProductId}", id);
        return true;
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        _logger.LogInformation("Recherche de produits avec le terme: {SearchTerm}", searchTerm);
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }
        
        return await _context.Products
            .Where(p => EF.Functions.Like(p.Name, $"%{searchTerm}%"))
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        _logger.LogInformation("Vérification de l'existence du produit: {ProductName}", name);
        
        var query = _context.Products.AsQueryable();
        
        query = query.Where(p => p.Name.ToLower() == name.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}
