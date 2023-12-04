using Microsoft.AspNetCore.Identity;

namespace TaniLink_Backend.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Picture { get; set; }
        public ICollection<Area> Areas { get; set; }
    }
}
