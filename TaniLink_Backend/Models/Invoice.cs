using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Invoice : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? PaymentEvidence { get; set; }
        public long TotalPrice { get; set; }
        public InvoiceStatus Status { get; set; }
        public ICollection<Order>? Orders { get; set; }

    }
}
