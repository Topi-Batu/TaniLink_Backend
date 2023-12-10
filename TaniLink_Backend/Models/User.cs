using Microsoft.AspNetCore.Identity;

namespace TaniLink_Backend.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Picture { get; set; }
        public string? Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public long Xp { get; set; }
        public string? ConnectionId { get; set; }
        public ICollection<Area>? Areas { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Seller>? Sellers { get; set; }
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
        public ICollection<ConversationMember>? ConversationMembers { get; set; }
        public ICollection<Message>? Messages { get; set; }


    }
}
