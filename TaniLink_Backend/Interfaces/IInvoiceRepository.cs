using TaniLink_Backend.Models;

namespace TaniLink_Backend.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> CreateInvoice(Invoice invoice);
        Task<Invoice> DeleteInvoice(string invoiceId);
        Task<IEnumerable<Invoice>> GetAllInvoices();
        Task<IEnumerable<Invoice>> GetAllInvoicesByUser(string userId);
        Task<Invoice> GetInvoiceById(string invoiceId);
        Task<Invoice> UpdateInvoice(Invoice invoice);
    }
}
