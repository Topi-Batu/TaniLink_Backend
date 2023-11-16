using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Area
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Provinsi { get; set; }
        public string? Kota { get; set; }
        public string? Kecamatan { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
    }
}
