using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Order : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
        public long DeliveryPrice { get; set; }
        public int Weight { get; set; }
        public string? CourierName { get; set; }
        public string? CourierPhoneNumber { get; set; }
        public Address? Address { get; set; }
        public ICollection<ShoppingCart>? ShoppingCart { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }

    }
}
