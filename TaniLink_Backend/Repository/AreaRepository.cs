using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Area> CreateArea(Area area)
        {
            var createArea = _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return createArea.Entity;
        }

        public async Task<Area> DeleteArea(string areaId)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == areaId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Area)))
            {
                (area as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Areas.Attach(area);
                _context.Entry(area).State = EntityState.Modified;
            }
            else
            {
                _context.Areas.Remove(area);
            }

            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<IEnumerable<Area>> GetAllAreas()
        {
            var areas = await _context.Areas.ToListAsync();
            return areas;
        }

        public async Task<Area> GetAreaById(string areaId)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaId);
            return area;
        }

        public async Task<Area> UpdateArea(Area area)
        {
            var updateArea = _context.Areas.Update(area);
            await _context.SaveChangesAsync();
            return updateArea.Entity;
        }
    }
}
