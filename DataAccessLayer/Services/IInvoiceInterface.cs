using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public interface IInvoiceInterface
    {
        Task AddInvoice(Invoice invoice);

        Task<Invoice> FindInvoiceNumberAsync(string invoiceNumber);

        Invoice GetInvoiceById(string invoiceId);

        List<Invoice> ListAsync();
        //bool ItemExist(string itemcode);
    }
}
