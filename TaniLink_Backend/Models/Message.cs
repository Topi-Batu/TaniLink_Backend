using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Message : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? ChatMessage { get; set; }
        public int Status { get; set; }
        public User? User { get; set; }
        public Conversation? Conversation { get; set; }
        public ICollection<MessageImage>? Images { get; set; }

    }
}
