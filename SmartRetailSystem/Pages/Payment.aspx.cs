using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using SmartRetailSystem.DAL;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class Payment : Page
    {
        PaymentService service = new PaymentService();
        CartService cartService = new CartService();
        CustomerService customerService = new CustomerService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 1. ඊළඟට ලැබෙන Order ID එක පෙන්වීම
                try
                {
                    int nextId = service.GetNextOrderID();
                    lblOrderID.Text = "ORD-" + nextId.ToString("D3");
                }
                catch { lblOrderID.Text = "ORD-XXX"; }

                LoadPaymentDetails();
            }
        }

        private void LoadPaymentDetails()
        {
            if (Session["CustomerID"] != null && Session["GrandTotal"] != null)
            {
                decimal grandTotal = Convert.ToDecimal(Session["GrandTotal"]);
                txtPaymentAmount.Text = grandTotal.ToString("N2");

                ddlPaymentMethod.Items.Clear();
                ddlPaymentMethod.Items.Add(new ListItem("Cash On Delivery", "Cash"));
                ddlPaymentMethod.Items.Add(new ListItem("Card Payment", "Card"));
                ddlPaymentMethod.Items.Add(new ListItem("Bank Transfer", "Bank"));
            }
            else
            {
                Response.Redirect("CreateOrder.aspx");
            }
        }

        protected void btnRecordPayment_Click(object sender, EventArgs e)
        {
            try
            {
                int customerId = Convert.ToInt32(Session["CustomerID"]);
                decimal paymentAmount = Convert.ToDecimal(txtPaymentAmount.Text);
                decimal subTotal = Convert.ToDecimal(Session["SubTotal"] ?? 0);
                decimal tax = Convert.ToDecimal(Session["Tax"] ?? 0);
                decimal grandTotal = Convert.ToDecimal(Session["GrandTotal"] ?? 0);
                string selectedMethod = ddlPaymentMethod.SelectedValue;

                // --- 1. Credit Limit Validation (Card Payment වලට හැර අනෙක් ඒවාට පමණයි) ---
                if (selectedMethod != "Card") // මෙතැනදී Card Payment නොවන්නේ නම් පමණක් ලිමිට් එක බලයි
                {
                    var customer = customerService.GetCustomerById(customerId);
                    if (customer != null)
                    {
                        decimal availableCredit = customer.CreditLimit - customer.OutstandingBalance;
                        if (grandTotal > availableCredit)
                        {
                            string msg = $"Action Denied! Grand Total exceeds available Credit Limit.\\nAvailable: Rs. {availableCredit:N2}";
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{msg}');", true);
                            return;
                        }
                    }
                }

                // --- 2. Card Details Validation ---
                string status = "Pending";
                if (selectedMethod == "Card")
                {
                    if (string.IsNullOrEmpty(txtCardNumber.Text) || string.IsNullOrEmpty(txtCardExpiry.Text))
                    {
                        lblStatus.Text = "Please enter valid Card Details.";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                    // කාඩ් එකෙන් ගෙවන නිසා වහාම "Paid" ලෙස සලකුණු වේ
                    status = "Paid";
                }

                // --- 3. Models Setup ---
                OrderModel order = new OrderModel
                {
                    CustomerID = customerId,
                    SubTotal = subTotal,
                    Tax = tax,
                    GrandTotal = grandTotal,
                    PaymentStatus = status,
                    DeliveryStatus = "Pending",
                    Items = (List<OrderItemModel>)Session["CartItems"]
                };

                PaymentModel payment = new PaymentModel
                {
                    CustomerID = customerId,
                    PaidAmount = (status == "Paid") ? paymentAmount : 0,
                    PaymentMethod = selectedMethod
                };

                // --- 4. Database Insert ---
                int orderIdFromDB = service.AddPayment(order, payment);

                if (orderIdFromDB > 0)
                {
                    // --- 5. වැදගත්: ගෙවීම් කළ අයිතම පමණක් Cart එකෙන් ඉවත් කිරීම ---
                    if (Session["SelectedProductIds"] != null)
                    {
                        List<int> selectedProductIds = (List<int>)Session["SelectedProductIds"];
                        cartService.RemoveSelectedItemsFromCart(customerId, selectedProductIds);

                        // පාවිච්චි කළ Session ඉවත් කිරීම
                        Session["SelectedProductIds"] = null;
                        Session["CartItems"] = null;
                    }

                    // --- 6. පාරිභෝගිකයාගේ Balance එක Update කිරීම ---
                    service.UpdateCustomerOutstandingBalance(customerId);

                    // --- 7. සාර්ථක පණිවිඩය පෙන්වීම ---
                    string finalOrderID = "ORD-" + orderIdFromDB.ToString("D3");
                    string successMsg = $"Success! Your Order ID is {finalOrderID}";

                    string script = $"alert('{successMsg}'); window.location='CustomerDashboard.aspx';";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
                }
                else
                {
                    lblStatus.Text = "Order processing failed.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateOrder.aspx");
        }
    }
}