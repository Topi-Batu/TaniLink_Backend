using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Commodity : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Area>? Areas { get; set; }
        public ICollection<Product>? Products { get; set; }

    }
}
