using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Prediction : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public DateOnly Date { get; set; }
        public string? Price { get; set; }
        public Commodity? Commodity { get; set; }
        public ICollection<Area>? Areas { get; set; }

    }
}
