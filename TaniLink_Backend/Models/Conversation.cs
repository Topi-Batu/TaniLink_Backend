using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Conversation : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Type { get; set; }
        public ICollection<ConversationMember>? ConversationMembers { get; set; }
        public ICollection<Message>? Messages { get; set; }


    }
}
