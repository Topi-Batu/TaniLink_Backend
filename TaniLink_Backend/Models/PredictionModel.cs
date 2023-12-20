using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class PredictionModel : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? ModelLink { get; set; }
        public bool IsUsed { get; set; }
        public Commodity? Commodity { get; set; }
    }
}
