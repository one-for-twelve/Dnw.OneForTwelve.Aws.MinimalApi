namespace Shared.Models
{
    public class ProductWrapper
    {
        public ProductWrapper(IEnumerable<Product> products)
        {
            Products = products.ToList();
        }
        
        public List<Product> Products { get; set; }
    }
}