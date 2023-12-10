using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Seller : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? NoKtp { get; set; }
        public string? FotoKtp { get; set; }
        public string? NoRek { get; set; }
        public string? FotoBuktab { get; set; }
        public int Role { get; set; }
        public User? User { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
