using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAddresses();
        Task<Address> GetAddressById(string addressId);
        Task<Address> CreateAddress(Address address);
        Task<Address> UpdateAddress(Address address);
        Task<Address> DeleteAddress(string addressId);
    }
}
