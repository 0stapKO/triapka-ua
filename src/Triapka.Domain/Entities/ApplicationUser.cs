using Microsoft.AspNetCore.Identity;

namespace Triapka.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public bool IsSubscribedToNewsletter { get; set; } = false;

        public virtual Cart? Cart { get; set; }

        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = [];

        public virtual ICollection<Order> Orders { get; set; } = [];
    }
}
