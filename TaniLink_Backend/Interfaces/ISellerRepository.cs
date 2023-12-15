using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface ISellerRepository
    {
        Task<IEnumerable<Seller>> GetAllSellers();
        Task<Seller> GetSellerById(string sellerId);
        Task<Seller> CreateSeller(Seller seller);
        Task<Seller> UpdateSeller(Seller seller);
        Task<Seller> DeleteSeller(string sellerId);
    }
}
