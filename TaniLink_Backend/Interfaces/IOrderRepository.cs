using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order> GetOrderById(string orderId);
        Task<IEnumerable<Order>> GetOrdersBySellerId(string sellerId);
        Task<IEnumerable<Order>> GetOrdersByBuyerId(string buyerId);
        Task<IEnumerable<Order>> GetOrdersByProductId(string productId);
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(Order order);
        Task<Order> DeleteOrder(string orderId);
    }
}
