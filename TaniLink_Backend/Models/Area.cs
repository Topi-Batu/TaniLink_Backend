using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Area : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Provinsi { get; set; }
        public string? Kota { get; set; }
        public string? Kecamatan { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<Commodity>? Commodities { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<Prediction>? Predictions { get; set; }
    }
}
