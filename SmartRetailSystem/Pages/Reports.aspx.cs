using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class Reports : Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCustomers();
                LoadProducts();
            }
        }

        private void LoadCustomers()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT CustomerID, Name FROM Customers ORDER BY Name", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlCustomer.DataSource = dt;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();

                // ListItem සඳහා සම්පූර්ණ namespace එක භාවිතා කර ඇත
                ddlCustomer.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--All Customers--", "0"));
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT ProductID, ProductName FROM Products ORDER BY ProductName", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlProduct.DataSource = dt;
                ddlProduct.DataTextField = "ProductName";
                ddlProduct.DataValueField = "ProductID";
                ddlProduct.DataBind();

                // ListItem සඳහා සම්පූර්ණ namespace එක භාවිතා කර ඇත
                ddlProduct.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--All Products--", "0"));
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            string query = @"
                SELECT o.OrderID, c.Name AS Customer, o.OrderDate, o.SubTotal, o.GrandTotal, o.PaymentStatus, o.DeliveryStatus
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                WHERE 1=1";

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(connStr);

                if (!string.IsNullOrEmpty(txtFromDate.Text))
                {
                    query += " AND o.OrderDate >= @FromDate ";
                    cmd.Parameters.AddWithValue("@FromDate", txtFromDate.Text);
                }

                if (!string.IsNullOrEmpty(txtToDate.Text))
                {
                    query += " AND o.OrderDate <= @ToDate ";
                    cmd.Parameters.AddWithValue("@ToDate", txtToDate.Text);
                }

                if (ddlCustomer.SelectedValue != "0")
                {
                    query += " AND o.CustomerID = @CustomerID ";
                    cmd.Parameters.AddWithValue("@CustomerID", ddlCustomer.SelectedValue);
                }

                if (ddlProduct.SelectedValue != "0")
                {
                    query += " AND EXISTS (SELECT 1 FROM OrderItems oi WHERE oi.OrderID=o.OrderID AND oi.ProductID=@ProductID)";
                    cmd.Parameters.AddWithValue("@ProductID", ddlProduct.SelectedValue);
                }

                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReports.DataSource = dt;
                gvReports.DataBind();
            }
        }

        // Export to Excel
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (gvReports.Rows.Count == 0) return;

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=Report.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvReports.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        // Export to PDF using iTextSharp
        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (gvReports.Rows.Count == 0) return;

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            PdfPTable pdfTable = new PdfPTable(gvReports.HeaderRow.Cells.Count);

            // Add headers
            foreach (TableCell headerCell in gvReports.HeaderRow.Cells)
            {
                PdfPCell cell = new PdfPCell(new Phrase(headerCell.Text));
                cell.BackgroundColor = new BaseColor(240, 240, 240); // LIGHT_GRAY
                pdfTable.AddCell(cell);
            }

            // Add data rows
            foreach (GridViewRow row in gvReports.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    // HTML tags ඉවත් කර පිරිසිදු පෙළ පමණක් ලබා ගැනීමට (උදා: &nbsp;)
                    string cellText = HttpUtility.HtmlDecode(cell.Text).Trim();
                    pdfTable.AddCell(new Phrase(cellText));
                }
            }

            pdfDoc.Add(pdfTable);
            pdfDoc.Close();

            Response.Write(pdfDoc);
            Response.End();
        }

        // Required for exporting GridView
        public override void VerifyRenderingInServerForm(Control control)
        {
            // GridView එක අපනයනය කිරීමට මෙය හිස්ව තැබීම අනිවාර්ය වේ.
        }

        protected void gvReports_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}