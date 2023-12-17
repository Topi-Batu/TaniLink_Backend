using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts();
        Task<IEnumerable<ShoppingCart>> GetAllShoppingCartsByUser(string userId);
        Task<ShoppingCart> GetShoppingCartById(string shoppingCartId);
        Task<ShoppingCart> CreateShoppingCart(ShoppingCart shoppingCart);
        Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart);
        Task<ShoppingCart> DeleteShoppingCart(string shoppingCartId);
    }
}
