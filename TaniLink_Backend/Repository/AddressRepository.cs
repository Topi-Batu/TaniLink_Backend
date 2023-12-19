using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Address> CreateAddress(Address address)
        {
            var createAddress = _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return createAddress.Entity;
        }

        public async Task<Address> DeleteAddress(string addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Address)))
            {
                (address as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Addresses.Attach(address);
                _context.Entry(address).State = EntityState.Modified;
            }
            else
            {
                _context.Addresses.Remove(address);
            }

            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> GetAddressById(string addressId)
        {
            var address = await _context.Addresses
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == addressId);
            return address;
        }

        public async Task<IEnumerable<Address>> GetAllAddresses()
        {
            var addresses = await _context.Addresses
                .Include(a => a.User)
                .ToListAsync();
            return addresses;
        }

        public async Task<Address> UpdateAddress(Address address)
        {
            var updateAddress = _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return updateAddress.Entity;
        }
    }
}
