using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SmartRetailSystem.Services;
using SmartRetailSystem.Models;
using SmartRetailSystem.Classes;

namespace SmartRetailSystem
{
    public partial class CreateOrder : Page
    {
        private CartService cartService = new CartService();
        private ProductService productService = new ProductService();
        private CustomerService customerService = new CustomerService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CheckUserRole();
                LoadProductsGrid();
                RefreshUI();
            }
        }

        private void CheckUserRole()
        {
            string role = Session["Role"]?.ToString();

            if (role == "Admin" || role == "Staff")
            {
                phAdminView.Visible = true;
                LoadCustomerList();
                lblCustomerName.Text = "Please select a customer";
            }
            else
            {
                phAdminView.Visible = false;
                int customerId = Convert.ToInt32(Session["CustomerID"] ?? 0);

                var cust = customerService.GetCustomerById(customerId);
                if (cust != null)
                {
                    UpdateCustomerDisplay(cust);
                }
                else
                {
                    lblCustomerName.Text = Session["Username"]?.ToString();
                }
                CheckAccountStatus();
            }
        }

        private void CheckAccountStatus()
        {
            bool isBlocked = Session["Status"]?.ToString() == "Blocked";

            if (isBlocked)
            {
                btnConfirmOrder.Enabled = false;
                btnConfirmOrder.Style.Add("background", "linear-gradient(135deg, #94a3b8, #64748b)");
                btnConfirmOrder.ToolTip = "Your account is blocked.";

                lblMessage.Text = "Your account is currently blocked. Order functionality is disabled. Please contact support.";
                pnlBlockMessage.Visible = true;

                foreach (RepeaterItem item in rptProducts.Items)
                {
                    Button btnAdd = (Button)item.FindControl("btnAdd");
                    Button btnDirect = (Button)item.FindControl("btnDirect");
                    if (btnAdd != null) btnAdd.Enabled = false;
                    if (btnDirect != null)
                    {
                        btnDirect.Enabled = false;
                        btnDirect.Style.Add("opacity", "0.5");
                    }
                }
            }
            else
            {
                pnlBlockMessage.Visible = false;
                btnConfirmOrder.Enabled = true;
                btnConfirmOrder.Style.Remove("background");
            }
        }

        private void LoadCustomerList()
        {
            var customers = customerService.GetAllCustomers();
            ddlCustomers.DataSource = customers;
            ddlCustomers.DataTextField = "Name";
            ddlCustomers.DataValueField = "CustomerID";
            ddlCustomers.DataBind();
            ddlCustomers.Items.Insert(0, new ListItem("-- Select Customer --", "0"));
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Convert.ToInt32(ddlCustomers.SelectedValue);
            if (selectedId > 0)
            {
                var cust = customerService.GetCustomerById(selectedId);
                if (cust != null)
                {
                    UpdateCustomerDisplay(cust);
                    RefreshUI();
                }
            }
        }

        private void UpdateCustomerDisplay(SmartRetailSystem.Classes.Customer cust)
        {
            Session["CustomerID"] = cust.CustomerID;
            Session["CustomerName"] = cust.Name;
            Session["OutstandingBalance"] = cust.OutstandingBalance;
            Session["CreditLimit"] = cust.CreditLimit;
            Session["Status"] = cust.Status;

            lblCustomerName.Text = cust.Name;
            decimal available = cust.CreditLimit - cust.OutstandingBalance;

            lblAvailableCredit.Text = "Rs. " + (available > 0 ? available : 0).ToString("N2");
            lblAvailableCredit.ForeColor = available > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            CheckAccountStatus();
        }

        private void LoadProductsGrid()
        {
            var products = productService.GetAllProducts();
            rptProducts.DataSource = products;
            rptProducts.DataBind();
            CheckAccountStatus();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (Session["CustomerID"] == null || Session["CustomerID"].ToString() == "0")
            {
                Alert("Please select a customer first!");
                return;
            }

            if (Session["Status"]?.ToString() == "Blocked")
            {
                Alert("Action Denied! This account is Blocked.");
                return;
            }

            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            int productId = Convert.ToInt32(btn.CommandArgument);
            TextBox txtQty = (TextBox)item.FindControl("txtQty");

            if (int.TryParse(txtQty.Text, out int qty) && qty > 0)
            {
                cartService.AddToCart(Convert.ToInt32(Session["CustomerID"]), productId, qty);
                RefreshUI();
                txtQty.Text = "1";
            }
        }

        protected void btnDirectOrder_Click(object sender, EventArgs e)
        {
            if (Session["CustomerID"] == null || Session["CustomerID"].ToString() == "0")
            {
                Alert("Please select a customer first!");
                return;
            }

            if (Session["Status"]?.ToString() == "Blocked")
            {
                Alert("Action Denied! This account is Blocked.");
                return;
            }

            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            int pid = Convert.ToInt32(btn.CommandArgument);

            TextBox txtQty = (TextBox)item.FindControl("txtQty");
            int qty = int.TryParse(txtQty.Text, out int q) ? q : 1;

            var product = productService.GetProductById(pid);
            if (product != null)
            {
                decimal subtotal = product.Price * qty;
                decimal tax = subtotal * 0.10m;
                decimal grandTotal = subtotal + tax;

                Panel pnlSummary = (Panel)item.FindControl("pnlDirectSummary");
                Label lblDirectSub = (Label)item.FindControl("lblDirectSub");
                Label lblDirectTax = (Label)item.FindControl("lblDirectTax");
                Label lblDirectTotal = (Label)item.FindControl("lblDirectTotal");

                if (pnlSummary != null)
                {
                    lblDirectSub.Text = subtotal.ToString("N2");
                    lblDirectTax.Text = tax.ToString("N2");
                    lblDirectTotal.Text = grandTotal.ToString("N2");
                    pnlSummary.CssClass = "expand-panel active";

                    btn.Text = "Summary Loaded ✓";
                    btn.Style["background"] = "#0ea5e9";
                }

                Session["SubTotal"] = subtotal;
                Session["Tax"] = tax;
                Session["GrandTotal"] = grandTotal;

                var directItems = new List<OrderItemModel> {
                    new OrderItemModel { ProductID = pid, Quantity = qty, Price = product.Price }
                };
                Session["CartItems"] = directItems;

                // Direct Order නිසා එක් Product ID එකක් පමණක් ලිස්ට් එකට දමමු
                Session["SelectedProductIds"] = new List<int> { pid };
            }
        }

        protected void btnGoToPayment_Click(object sender, EventArgs e)
        {
            if (Session["CartItems"] != null)
            {
                Response.Redirect("Payment.aspx");
            }
            else
            {
                Alert("Session expired. Please try again.");
            }
        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            if (Session["Status"]?.ToString() == "Blocked") { Alert("Your account is blocked."); return; }

            List<OrderItemModel> selectedItems = new List<OrderItemModel>();
            List<int> selectedProductIds = new List<int>(); // මෙන්න මේ ලිස්ට් එක අලුතින් එකතු කළා
            decimal subtotalForSession = 0;

            foreach (GridViewRow row in gvCart.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk != null && chk.Checked)
                {
                    int pId = Convert.ToInt32(gvCart.DataKeys[row.RowIndex].Value);
                    int qty = Convert.ToInt32(row.Cells[2].Text);

                    string rawTotal = row.Cells[3].Text.Replace("Rs. ", "").Replace(",", "").Trim();
                    if (decimal.TryParse(rawTotal, out decimal rowTotal))
                    {
                        subtotalForSession += rowTotal;
                        selectedProductIds.Add(pId); // ID එක ලිස්ට් එකට දානවා

                        selectedItems.Add(new OrderItemModel
                        {
                            ProductID = pId,
                            Quantity = qty,
                            Price = rowTotal / qty
                        });
                    }
                }
            }

            if (selectedItems.Count == 0)
            {
                Alert("Please select items from the cart!");
                return;
            }

            decimal taxForSession = subtotalForSession * 0.10m;
            Session["SubTotal"] = subtotalForSession;
            Session["Tax"] = taxForSession;
            Session["GrandTotal"] = subtotalForSession + taxForSession;
            Session["CartItems"] = selectedItems;
            Session["SelectedProductIds"] = selectedProductIds; // මෙතැනදී Session එකට දත්ත ටික යනවා

            Response.Redirect("Payment.aspx");
        }

        private void RefreshUI()
        {
            if (Session["CustomerID"] != null)
            {
                int cId = Convert.ToInt32(Session["CustomerID"]);
                var items = cartService.GetCartItems(cId);
                gvCart.DataSource = items;
                gvCart.DataBind();
                litCartCount.Text = items.Count.ToString();
                UpdateSelectedTotals();
                UpdatePanelCart.Update();
                if (upBadge != null) upBadge.Update();
            }
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e) => UpdateSelectedTotals();

        private void UpdateSelectedTotals()
        {
            decimal selectedSubtotal = 0;

            foreach (GridViewRow row in gvCart.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk != null && chk.Checked)
                {
                    string totalStr = row.Cells[3].Text.Replace("Rs. ", "").Replace(",", "").Trim();
                    if (decimal.TryParse(totalStr, out decimal rowTotal))
                    {
                        selectedSubtotal += rowTotal;
                    }
                }
            }

            decimal tax = selectedSubtotal * 0.10m;
            decimal grandTotal = selectedSubtotal + tax;

            lblCartSubtotal.Text = selectedSubtotal.ToString("N2");
            lblCartTax.Text = tax.ToString("N2");
            lblGrandTotal.Text = grandTotal.ToString("N2");
        }

        protected void gvCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int productId = Convert.ToInt32(gvCart.DataKeys[index].Value);
                cartService.RemoveCartItem(Convert.ToInt32(Session["CustomerID"]), productId);
                RefreshUI();
            }
        }

        private void Alert(string msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{msg}');", true);
        }
    }
}