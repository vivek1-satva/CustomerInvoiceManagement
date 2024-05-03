using DataAccessLayer.Models;
using DataAccessLayer.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class InvoiceService : IInvoiceInterface
    {
        private IMongoCollection<Invoice> invoiceCollection;

        public InvoiceService(string connectionSring, string databaseName)
        {
            var mongoClient = new MongoClient(connectionSring);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            invoiceCollection = mongoDatabase.GetCollection<Invoice>("Invoices");
        }

        public async Task AddInvoice(Invoice invoice)
        {
            //invoice.LineItems.ForEach(p => p.SubTotal = p.Qty * p.UnitPrice);
            //double total = invoice.LineItems.Sum(item => item.UnitPrice * item.Qty);

            //double discountAmount = (total * invoice.Discount) / 100;

            //double netAmount = total - discountAmount + invoice.ShippingCharges;

            //invoice.Total = total;
            //invoice.DiscountAmount = discountAmount;
            //invoice.NetAmount = netAmount;
            await invoiceCollection.InsertOneAsync(invoice);


        }

        public Invoice GetInvoiceById(string invoiceId)
        {
            var objinvoice = invoiceCollection.Find(i => i.Id == invoiceId).FirstOrDefault();
            return objinvoice;
        }
        public List<Invoice> ListAsync()
        {
            return invoiceCollection.Find(_ => true).ToList();
        }
        public async Task<Invoice> FindInvoiceNumberAsync(string invoiceNumber)
        {
            return await invoiceCollection.Find(i => i.InvoiceNumber == invoiceNumber).FirstOrDefaultAsync();
        }

        //public bool ItemExist(string itemcode)
        //{
        //    var item = itemcollection.Find(i => i.ItemCode == itemcode).FirstOrDefault();
        //    if (item is null) return false; else return true;
        //}
    }
}
