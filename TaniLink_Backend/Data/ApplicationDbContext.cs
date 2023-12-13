using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMember> ConversationMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageImage> MessageImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Product)
                .WithMany(p => p.ShoppingCarts)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.ShoppingCarts)
                .OnDelete(DeleteBehavior.Restrict);
            /*modelBuilder.Entity<Order>()
                .HasMany(o => o.ShoppingCart)
                .WithMany(p => p.Orders)
                .UsingEntity(j => j.ToTable("OrderShoppingCart"));*/
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            foreach (var insertedEntry in insertedEntries)
            {
                var auditableEntity = insertedEntry as Auditable;
                if (auditableEntity != null)
                {
                    auditableEntity.CreatedAt = DateTimeOffset.Now;
                }
            }

            var modifiedEntries = this.ChangeTracker.Entries()
                       .Where(x => x.State == EntityState.Modified)
                       .Select(x => x.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                var auditableEntity = modifiedEntry as Auditable;
                if (auditableEntity != null)
                {
                    auditableEntity.UpdatedAt = DateTimeOffset.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
