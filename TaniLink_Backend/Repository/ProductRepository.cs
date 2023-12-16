using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;
using static Grpc.Core.Metadata;

namespace TaniLink_Backend.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            var createProduct = _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return createProduct.Entity;
        }

        public async Task<Product> DeleteProduct(string productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Product)))
            {
                (product as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Products.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
            }
            else
            {
                _context.Products.Remove(product);
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Area)
                .Include(p => p.Commodity)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
            return products;
        }

        public async Task<Product> GetProductById(string productId)
        {
            var product = await _context.Products
                .Include(p => p.Area)
                .Include(p => p.Commodity)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByAreaId(string areaId)
        {
            var product = await _context.Products
                .Where(p => p.Area.Id == areaId)
                .Include(p => p.Area)
                .Include(p => p.Commodity)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .ToListAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCommodityId(string commodityId)
        {
            var product = await _context.Products
                .Where(p => p.Commodity.Id == commodityId)
                .Include(p => p.Area)
                .Include(p => p.Commodity)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .ToListAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsBySellerId(string sellerId)
        {
            var product = await _context.Products
                .Where(p => p.Seller.Id == sellerId)
                .Include(p => p.Area)
                .Include(p => p.Commodity)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .ToListAsync();
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var updateProduct = _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return updateProduct.Entity;
        }
    }
}
