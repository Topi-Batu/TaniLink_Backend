using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class CommodityRepository : ICommodityRepository
    {
        private readonly ApplicationDbContext _context;

        public CommodityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Commodity> CreateCommodity(Commodity commodity)
        {
            var result = await _context.Commodities.AddAsync(commodity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Commodity> DeleteCommodity(string commodityId)
        {
            var commodity = await _context.Commodities
                .FirstOrDefaultAsync(c => c.Id == commodityId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Area)))
            {
                (commodity as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Commodities.Attach(commodity);
                _context.Entry(commodity).State = EntityState.Modified;
            }
            else
            {
                _context.Commodities.Remove(commodity);
            }

            await _context.SaveChangesAsync();
            return commodity;
        }

        public async Task<IEnumerable<Commodity>> GetAllCommodities()
        {
            var commodities = await _context.Commodities
                .Include(c => c.Areas)
                .ToListAsync();
            return commodities;
        }

        public async Task<Commodity> GetCommodityById(string commodityId)
        {
            var commodity = await _context.Commodities
                .Include(c => c.Areas)
                .Include(c => c.PredictionDatasets)
                .FirstOrDefaultAsync(c => c.Id == commodityId);
            return commodity;
        }

        public async Task<Commodity> UpdateCommodity(Commodity commodity)
        {
            var result = await _context.Commodities.AddAsync(commodity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}
