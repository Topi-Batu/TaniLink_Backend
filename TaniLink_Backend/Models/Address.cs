using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Address : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Detail { get; set; }
        public Area? Area { get; set; }
        public User? User { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
