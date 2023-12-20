using Microsoft.EntityFrameworkCore;
using TaniLink_Backend.Data;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Invoice> CreateInvoice(Invoice invoice)
        {
            var createInvoice = _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return createInvoice.Entity;
        }

        public async Task<Invoice> DeleteInvoice(string invoiceId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (typeof(Auditable).IsAssignableFrom(typeof(Invoice)))
            {
                (invoice as Auditable).DeletedAt = DateTimeOffset.UtcNow;
                _context.Invoices.Attach(invoice);
                _context.Entry(invoice).State = EntityState.Modified;
            }
            else
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Area)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Commodity)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Seller)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Images)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.Address)
                .ToListAsync();
            return invoices;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesByUser(string userId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.Orders.Select(o => o.ShoppingCart.Select(sc => sc.User.Id).FirstOrDefault()).FirstOrDefault() == userId)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Area)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Commodity)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Seller)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Images)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.Address)
                        .ThenInclude(a => a.User)
                .ToListAsync();
            return invoices;
        }

        public async Task<Invoice> GetInvoiceById(string invoiceId)
        {
            var invoice = await _context.Invoices
                .Where(i => i.Id == invoiceId)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Area)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Commodity)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Seller)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.ShoppingCart)
                        .ThenInclude(sc => sc.Product)
                            .ThenInclude(p => p.Images)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.Address)
                .FirstOrDefaultAsync();
            return invoice;
        }

        public async Task<Invoice> UpdateInvoice(Invoice invoice)
        {
            var updateInvoice = _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return updateInvoice.Entity;
        }
    }
}
