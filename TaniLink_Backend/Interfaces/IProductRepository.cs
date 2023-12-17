using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(string productId);
        Task<IEnumerable<Product>> GetProductsBySellerId(string sellerId);
        Task<IEnumerable<Product>> GetProductsByAreaId(string areaId);
        Task<IEnumerable<Product>> GetProductsByCommodityId(string commodityId);
        Task<IEnumerable<Product>> GetProductsBySearch(string search);
        Task<IEnumerable<Product>> GetProductByPriceRange(decimal minValue, decimal maxValue);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<Product> DeleteProduct(string productId);

    }
}
