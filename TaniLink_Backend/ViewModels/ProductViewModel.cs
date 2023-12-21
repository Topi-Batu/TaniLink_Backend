using System.ComponentModel.DataAnnotations;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public long Price { get; set; }
        [Required]
        public string? UnitName { get; set; }
        public string? Thumbnail { get; set; } = null;
        [Required]
        public int AvailableStock { get; set; }
        [Required]
        public string? CommodityId { get; set; }
        [Required]
        public string? AreaId { get; set; }
    }
}
