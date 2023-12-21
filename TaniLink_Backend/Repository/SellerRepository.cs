using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ApplicationDbContext _context;

        public SellerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Seller> CreateSeller(Seller seller)
        {
            var createSeller = _context.Sellers.Add(seller);
            await _context.SaveChangesAsync();
            return createSeller.Entity;
        }

        public async Task<Seller> DeleteSeller(string sellerId)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Id == sellerId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Seller)))
            {
                (seller as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Sellers.Attach(seller);
                _context.Entry(seller).State = EntityState.Modified;
            }
            else
            {
                _context.Sellers.Remove(seller);
            }

            await _context.SaveChangesAsync();
            return seller;
        }

        public async Task<IEnumerable<Seller>> GetAllSellers()
        {
            var sellers = await _context.Sellers
                .Include(s => s.User)
                .ToListAsync();
            return sellers;
        }

        public async Task<Seller> GetSellerById(string sellerId)
        {
            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sellerId);
            return seller;
        }

        public async Task<Seller> GetSellerByUserId(string userId)
        {
            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.User.Id == userId);
            return seller;
        }

        public async Task<Seller> UpdateSeller(Seller seller)
        {
            var updateSeller = _context.Sellers.Update(seller);
            await _context.SaveChangesAsync();
            return updateSeller.Entity;
        }
    }
}
