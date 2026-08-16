using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SmartRetailSystem
{
    public partial class Invoice : System.Web.UI.Page
    {
        // InvoiceService එක හරහා දත්ත ලබා ගැනීම
        InvoiceService invoiceService = new InvoiceService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // URL එකේ OrderID එක තිබේදැයි පරීක්ෂා කිරීම
                if (Request.QueryString["OrderID"] != null)
                {
                    if (int.TryParse(Request.QueryString["OrderID"], out int orderId))
                    {
                        LoadInvoice(orderId);
                    }
                }
                else
                {
                    SetDefaultLabels();
                }
            }
        }

        private void LoadInvoice(int orderId)
        {
            // Database එකෙන් පාරිභෝගික විස්තර සහිත Invoice model එක ලබා ගැනීම
            SmartRetailSystem.Models.Invoice inv = invoiceService.GetInvoice(orderId);

            if (inv != null)
            {
                // 1. ප්‍රධාන විස්තර
                lblInvoiceNumber.Text = "INV-" + inv.OrderID.ToString("D4");
                lblOrderDate.Text = inv.OrderDate.ToString("yyyy-MM-dd");
                lblCustomerName.Text = inv.CustomerName;

                // 2. ස්වයංක්‍රීයව පිරවෙන පාරිභෝගික විස්තර (Auto-fill from Database)
                // මෙහිදී Model එකේ ShippingAddress හෝ ContactNumber හිස් නම් පමණක් "N/A" පෙන්වයි
                lblShippingAddress.Text = !string.IsNullOrEmpty(inv.ShippingAddress) ? inv.ShippingAddress : "No Address Found";
                lblContact.Text = !string.IsNullOrEmpty(inv.ContactNumber) ? inv.ContactNumber : "No Contact Found";
                lblPaymentMethod.Text = !string.IsNullOrEmpty(inv.PaymentMethod) ? inv.PaymentMethod : "Online Payment";

                // 3. මුදල් ගණනය කිරීම් (Calculation Logic)
                decimal total = inv.GrandTotal;
                decimal paid = 0;
                decimal balance = 0;

                // Payment Status එක අනුව ගෙවූ මුදල තීරණය කිරීම
                if (inv.PaymentStatus == "Paid" || inv.PaidAmount >= total)
                {
                    paid = total;
                    balance = 0;
                    lblStatusBadge.Text = "PAID";
                    lblStatusBadge.CssClass = "badge bg-success status-badge text-decoration-none";
                }
                else
                {
                    paid = inv.PaidAmount;
                    balance = total - paid;
                    lblStatusBadge.Text = "PENDING";
                    lblStatusBadge.CssClass = "badge bg-warning text-dark status-badge text-decoration-none";
                }

                lblTotalAmount.Text = total.ToString("N2");
                lblPaidAmount.Text = paid.ToString("N2");
                lblBalance.Text = balance.ToString("N2");

                // 4. Grid එකට භාණ්ඩ ලැයිස්තුව ලබා දීම (With LINQ for Total)
                if (inv.Items != null && inv.Items.Count > 0)
                {
                    var gridData = inv.Items.Select(item => new {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Quantity * item.Price
                    }).ToList();

                    gvInvoiceDetails.DataSource = gridData;
                    gvInvoiceDetails.DataBind();
                }
            }
            else
            {
                SetDefaultLabels();
            }
        }

        private void SetDefaultLabels()
        {
            lblInvoiceNumber.Text = "N/A";
            lblCustomerName.Text = "N/A";
            lblOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            lblShippingAddress.Text = "N/A";
            lblContact.Text = "N/A";
            lblPaymentMethod.Text = "N/A";
            lblTotalAmount.Text = "0.00";
            lblPaidAmount.Text = "0.00";
            lblBalance.Text = "0.00";
        }

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            // PDF Generation Logic (iTextSharp භාවිතා කරන්නේ නම්)
            try
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + lblInvoiceNumber.Text + ".pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Document pdfDoc = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Simple PDF Content
                pdfDoc.Add(new Paragraph("SMART RETAIL - INVOICE"));
                pdfDoc.Add(new Paragraph("-------------------------------"));
                pdfDoc.Add(new Paragraph("Invoice: " + lblInvoiceNumber.Text));
                pdfDoc.Add(new Paragraph("Customer: " + lblCustomerName.Text));
                pdfDoc.Add(new Paragraph("Address: " + lblShippingAddress.Text));
                pdfDoc.Add(new Paragraph("Total: Rs. " + lblTotalAmount.Text));
                pdfDoc.Add(new Paragraph("-------------------------------"));
                pdfDoc.Add(new Paragraph("Thank you for your business!"));

                pdfDoc.Close();
                Response.Write(pdfDoc);
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }

        // GridView එක Render කිරීමේදී එන error වැලැක්වීමට
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Empty
        }
    }
}