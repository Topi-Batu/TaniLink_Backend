using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class PredictionDataset : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? DatasetLink { get; set; }
        public bool IsUsed { get; set; }
        public Commodity? Commodity { get; set; }
    }
}
