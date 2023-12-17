using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Order> CreateOrder(Order order)
        {
            var createOrder = _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return createOrder.Entity;
        }

        public async Task<Order> DeleteOrder(string orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Product)))
            {
                (order as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Orders.Attach(order);
                _context.Entry(order).State = EntityState.Modified;
            }
            else
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var orders  = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Invoices)
                .Include(o => o.ShoppingCart)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Invoices)
                .Include(o => o.ShoppingCart)
                .FirstOrDefaultAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByBuyerId(string buyerId)
        {
            var orders = await _context.Orders
                .Where(o => o.Address.User.Id == buyerId)
                .Include(o => o.Address)
                    .ThenInclude(sc => sc.User)
                .Include(o => o.Invoices)
                .Include(o => o.ShoppingCart)
                    .ThenInclude(sc => sc.Product)
                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetOrdersByProductId(string productId)
        {
            var orders = await _context.Orders
                .Where(o => o.ShoppingCart.Any(sc => sc.Product.Id == productId))
                .Include(o => o.Address)
                .Include(o => o.Invoices)
                .Include(o => o.ShoppingCart)
                    .ThenInclude(sc => sc.Product)
                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerId(string sellerId)
        {
            var orders = await _context.Orders
                .Where(o => o.ShoppingCart.Any(sc => sc.Product.Seller.User.Id == sellerId ))
                .Include(o => o.Address)
                .Include(o => o.Invoices)
                .Include(o => o.ShoppingCart)
                    .ThenInclude(sc => sc.Product)
                        .ThenInclude(p => p.Seller)
                            .ThenInclude(p => p.User)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            var updateOrder = _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return updateOrder.Entity;
        }
    }
}
