using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Order : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public int Amount { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public Address? Address { get; set; }
    }
}
