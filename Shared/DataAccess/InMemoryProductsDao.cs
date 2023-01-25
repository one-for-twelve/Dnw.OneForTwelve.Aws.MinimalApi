using Shared.Models;

namespace Shared.DataAccess;

public class InMemoryProductsDao : IProductsDao
{
    private static readonly Dictionary<string, Product> Products = new();

    public Task<Product?> GetProduct(string id)
    {
        Products.TryGetValue(id, out var product);
        return Task.FromResult(product ?? null);
    }

    public Task PutProduct(Product product)
    {
        if (!ProductExists(product.Id))
        {
            Products.Add(product.Id, product);
        }

        return Task.CompletedTask;
    }

    public Task DeleteProduct(string id)
    {
        if (ProductExists(id))
        {
            Products.Remove(id);
        }

        return Task.CompletedTask;
    }

    public Task<ProductWrapper> GetAllProducts()
    {
        return Task.FromResult(new ProductWrapper(Products.Values));
    }
    
    private static bool ProductExists(string id)
    {
        return Products.ContainsKey(id);
    }
}