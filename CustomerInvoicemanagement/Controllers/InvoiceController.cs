using DataAccess.ViewModel;
using DataAccessLayer.Models;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using static Amazon.Runtime.Internal.Settings.SettingsCollection;
using System.Text;
using InvoiceCustomerAuthAPI.Utitlity;
using DinkToPdf;
using APICRUD_mongodb.CommonjsonResponse;

namespace CustomerInvoiceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceInterface invoiceInterface;
        private readonly IMongoCollection<DynamicJsonResponse> invoiceCollection;

        public InvoiceController(IInvoiceInterface invoiceInterface, IMongoCollection<DynamicJsonResponse> invoiceCollection)
        {
            this.invoiceInterface = invoiceInterface;
            this.invoiceCollection = invoiceCollection;
        }

        [HttpPost]
        [Route("CreateInvoice")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<ActionResult> Post(Invoice invoice)
        {
            var objCommonJson = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    var existingItem = await invoiceInterface.FindInvoiceNumberAsync(invoice.InvoiceNumber);
                    if (existingItem != null)
                    {
                        objCommonJson.ResponseStatus = 0;
                        objCommonJson.Message = "Invoice number must be unique";
                        return Ok(objCommonJson);
                    }
                    //foreach (var invoiceItem in invoice.LineItems)
                    //{
                    //    if (!invoiceInterface.ItemExist(invoiceItem.ItemCode))
                    //    {
                    //        objCommonJson.ResponseStatus = 0;
                    //        objCommonJson.Message = "Item not exist";
                    //        return Ok(objCommonJson);
                    //    }
                    //}
                    await invoiceInterface.AddInvoice(invoice);
                    var dynamicResponse = MapInvoiceToDynamicResponse(invoice);
                    objCommonJson.ResponseStatus = 1;
                    objCommonJson.Message = "Invoice added successfully!";
                    objCommonJson.Result = dynamicResponse;
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Any())
                                           .ToDictionary(
                                                kvp => kvp.Key,
                                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                            );
                    objCommonJson.ResponseStatus = 0;
                    objCommonJson.Message = "Validation failed. Please check the errors.";
                    objCommonJson.Result = errors;
                }
            }
            catch (Exception ex)
            {
                objCommonJson.ResponseStatus = 0;
                objCommonJson.Message = ex.Message;

                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    objCommonJson.Message = ex.InnerException.Message;
                }
            }
            return Ok(objCommonJson);
        }

        private DynamicJsonResponse MapInvoiceToDynamicResponse(Invoice invoice)
        {
            if (invoice == null)
            {
                return null;
            }


            var dynamicResponse = new DynamicJsonResponse
            {

                CustomerId = invoice.CustomerId,
                LineItems = invoice.LineItems.Select(item => new ItemInvoice
                {

                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ProductDescription = item.ProductDescription,
                    UnitPrice = item.UnitPrice,
                    Qty = item.Qty,
                    SubTotal = item.Qty * item.UnitPrice
                }).ToList(),
                Total = (double)invoice.Total,
                Discount = invoice.Discount,
                DiscountAmount = (double)invoice.DiscountAmount,
                ShippingCharges = invoice.ShippingCharges,
                NetAmount = (double)invoice.NetAmount
            };

            return dynamicResponse;
        }

        //[HttpGet]
        //[Route("generatenvoicePdf/{id}")]
        //public async Task<IActionResult> DownloadPdf(string id)
        //{
        //    var converter = new SynchronizedConverter(new PdfTools());

        //    //get invoice 
        //    var objInvoice = invoiceInterface.GetInvoiceById(id);

        //    if (objInvoice != null && !string.IsNullOrEmpty(objInvoice.Id))
        //    {
        //        //read html page
        //        string invoiceHtml = System.IO.File.ReadAllText("C:\\Users\\chint\\source\\repos\\API\\InvoiceCustomerAuthAPI\\InvoiceCustomerAuthAPI\\InvoiceTemplate\\Invoicetemplate.html");

        //        var shippingaddress = new StringBuilder();
        //        shippingaddress.Append("<strong>");
        //        shippingaddress.Append(objInvoice.ShippingAddress.StreetAddress);
        //        shippingaddress.Append("</strong>");
        //        shippingaddress.Append("<p>");
        //        shippingaddress.Append(objInvoice.ShippingAddress.Country);
        //        shippingaddress.Append("</p>");
        //        shippingaddress.Append("<p>");
        //        shippingaddress.Append(objInvoice.ShippingAddress.State);
        //        shippingaddress.Append("</p>");
        //        shippingaddress.Append("<p>");
        //        shippingaddress.Append(objInvoice.ShippingAddress.City + " " + objInvoice.ShippingAddress.PostalCode);
        //        shippingaddress.Append("</p>");

        //        invoiceHtml = invoiceHtml.Replace("{shippingAddress}", shippingaddress.ToString());

        //        var billingaddress = new StringBuilder();
        //        billingaddress.Append("<strong>");
        //        billingaddress.Append(objInvoice.ShippingAddress.StreetAddress);
        //        billingaddress.Append("</strong>");
        //        billingaddress.Append("<p>");
        //        billingaddress.Append(objInvoice.ShippingAddress.Country);
        //        billingaddress.Append("</p>");
        //        billingaddress.Append("<p>");
        //        billingaddress.Append(objInvoice.ShippingAddress.State);
        //        billingaddress.Append("</p>");
        //        billingaddress.Append("<p>");
        //        billingaddress.Append(objInvoice.ShippingAddress.City + " " + objInvoice.ShippingAddress.PostalCode);
        //        billingaddress.Append("</p>");

        //        invoiceHtml = invoiceHtml.Replace("{BillingAddress}", billingaddress.ToString());

        //        var lineitem = new StringBuilder();
        //        foreach (var items in objInvoice.LineItems)
        //        {

        //            lineitem.Append("<tr>");
        //            lineitem.AppendLine("<td>" + items.ItemCode + "</td>");
        //            lineitem.AppendLine("<td>" + items.ItemName + "</td>");
        //            lineitem.AppendLine("<td>" + items.Qty + "</td>");
        //            lineitem.AppendLine("<td>" + items.UnitPrice + "</td>");
        //            lineitem.AppendLine("<td>" + items.SubTotal + "</td>");
        //            lineitem.Append("</tr>");
        //        }
        //        invoiceHtml = invoiceHtml.Replace("{lineItems}", lineitem.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{InvoiceNumber}", objInvoice.InvoiceNumber);
        //        invoiceHtml = invoiceHtml.Replace("{InvoiceDate}", objInvoice.InvoiceDate.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{DueDate}", objInvoice.DueDate.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{Total}", objInvoice.Total.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{Discount}", objInvoice.Discount.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{ShippingCharges}", objInvoice.ShippingCharges.ToString());
        //        invoiceHtml = invoiceHtml.Replace("{NetAmount}", objInvoice.NetAmount.ToString());

        //        var doc = new HtmlToPdfDocument()
        //        {
        //            GlobalSettings = {
        //             ColorMode = ColorMode.Color,
        //             Orientation = Orientation.Portrait,
        //             PaperSize = PaperKind.A4,
        //             Margins = new MarginSettings() { Top = 10 },
        //        },
        //            Objects = {
        //            new ObjectSettings()
        //        {
        //            HtmlContent = invoiceHtml,
        //            PagesCount = true,
        //            WebSettings = { DefaultEncoding = "utf-8"},
        //            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }

        //        },
        //    }
        //        };
        //        byte[] pdfile = converter.Convert(doc);

        //        return File(pdfile, "application/octet-stream", id + ".pdf");
        //    }
        //    else
        //    {
        //        return Ok("Invoice not found!");
        //    }
        //}


        //[HttpGet]
        //[Route("exporttoCSV")]
        //public async Task<ActionResult> ExporttoCSV()
        //{


        //    // get all invoices
        //    var objInvoices = invoiceInterface.ListAsync();

        //    var objnew = objInvoices.SelectMany(objInvoice => objInvoice.LineItems.Select(objLineItem => new CustomerInvoiceViewModel
        //    {
        //        ItemCode = objLineItem.ItemCode,
        //        ProductDescription = objLineItem.ProductDescription,
        //        UnitPrice = objLineItem.UnitPrice,
        //        Qty = objLineItem.Qty,
        //        SubTotal = objLineItem.SubTotal,
        //        InvoiceNumber = objInvoice.InvoiceNumber,
        //        CustomerId = objInvoice.CustomerId,
        //        Id = objInvoice.Id,
        //        InvoiceDate = objInvoice.InvoiceDate,
        //        DueDate = objInvoice.DueDate,
        //        NetAmount = objInvoice.NetAmount,
        //        Discount = objInvoice.Discount,
        //        Total = objInvoice.Total,
        //        DiscountAmount = objInvoice.DiscountAmount,
        //        ShippingCharges = objInvoice.ShippingCharges,
        //        BillingStreetAddress = objInvoice.BillingAddress.StreetAddress,
        //        BillingCity = objInvoice.BillingAddress.City,
        //        BillingState = objInvoice.BillingAddress.State,
        //        BillingCountry = objInvoice.BillingAddress.Country,
        //        BillingPostalCode = objInvoice.BillingAddress.PostalCode,
        //        ShippingStreetAddress = objInvoice.BillingAddress.StreetAddress,
        //        ShippingCity = objInvoice.BillingAddress.City,
        //        ShippingCountry = objInvoice.BillingAddress.Country,
        //        ShippingPostalCode = objInvoice.BillingAddress.PostalCode,
        //        ShippingState = objInvoice.BillingAddress.State,
        //        //Tot = objInvoice.SubTotal
        //    })).ToList();

        //    // convert list to CSV string
        //    var csvInvoiceString = CommonFunction.ToCsv(objnew);

        //    // return CSV file
        //    byte[] csv = Encoding.UTF8.GetBytes(csvInvoiceString);
        //    return File(csv, "application/octet-stream", "Invoices.csv");
        //}

    }
}
