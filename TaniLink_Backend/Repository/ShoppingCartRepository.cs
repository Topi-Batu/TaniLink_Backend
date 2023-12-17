using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ShoppingCart> CreateShoppingCart(ShoppingCart shoppingCart)
        {
            var createShoppingCart = _context.ShoppingCarts.Add(shoppingCart);
            await _context.SaveChangesAsync();
            return createShoppingCart.Entity;
        }

        public async Task<ShoppingCart> DeleteShoppingCart(string shoppingCartId)
        {
            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Product)))
            {
                (shoppingCart as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.ShoppingCarts.Attach(shoppingCart);
                _context.Entry(shoppingCart).State = EntityState.Modified;
            }
            else
            {
                _context.ShoppingCarts.Remove(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return shoppingCart;
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts()
        {
            var shoppingCarts  = await _context.ShoppingCarts
                .Include(sc => sc.Product)
                .ToListAsync();
            return shoppingCarts;
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUser(string userId)
        {
            var shoppingCarts = await _context.ShoppingCarts
                .Where(sc => sc.User.Id == userId)
                .Include(sc => sc.User)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p.Area)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p.Commodity)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p.Seller)
                .Include(sc => sc.Product)
                    .ThenInclude(p => p.Images)
                .Include(sc => sc.Orders)
                .ToListAsync();
            return shoppingCarts;
        }

        public async Task<ShoppingCart> GetShoppingCartById(string shoppingCartId)
        {
            var shoppingCart = await _context.ShoppingCarts
                .Where(sc => sc.Id == shoppingCartId)
                .Include(sc => sc.Product)
                .FirstOrDefaultAsync();
            return shoppingCart;
        }

        public async Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            var updateShoppingCart = _context.ShoppingCarts.Update(shoppingCart);
            await _context.SaveChangesAsync();
            return updateShoppingCart.Entity;
        }
    }
}
