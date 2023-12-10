using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class ShoppingCart : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public int Amount { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
        public ICollection<Order>? Orders { get; set; }

    }
}
