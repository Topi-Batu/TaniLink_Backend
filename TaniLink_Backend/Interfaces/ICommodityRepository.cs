using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface ICommodityRepository
    {
        Task<IEnumerable<Commodity>> GetAllCommodities();
        Task<Commodity> GetCommodityById(string commodityId);
        Task<Commodity> CreateCommodity(Commodity commodity);
        Task<Commodity> UpdateCommodity(Commodity commodity);
        Task<Commodity> DeleteCommodity(string commodityId);
    }
}
