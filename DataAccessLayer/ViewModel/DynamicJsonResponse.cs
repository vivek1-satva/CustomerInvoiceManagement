using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel
{
    public class DynamicJsonResponse
    {
        public string CustomerId { get; set; }
      //  public List<ItemInvoice> LineItems { get; set; }

        public double Total { get; set; }


        public int Discount { get; set; }

        public double DiscountAmount { get; set; }

        public double ShippingCharges { get; set; }

        public double NetAmount { get; set; }
    }
}
