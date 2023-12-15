using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAreas();
        Task<Area> GetAreaById(string areaId);
        Task<Area> CreateArea(Area area);
        Task<Area> UpdateArea(Area area);
        Task<Area> DeleteArea(string areaId);
    }
}
