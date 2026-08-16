<%@ Page Title="My Orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerOrders.aspx.cs" Inherits="SmartRetailSystem.CustomerOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body { font-family: 'Poppins', sans-serif; background-color: #f4f7f6; }
        .main-container { padding: 2rem 5% !important; }
        .page-content-card {
            background: #ffffff; border-radius: 20px; padding: 2.5rem;
            box-shadow: 0 10px 30px rgba(0,0,0,0.05); border: 1px solid #eee; margin-top: 20px;
        }
        .order-title {
            font-size: 2.2rem; font-weight: 700; color: #2c3e50; margin-bottom: 2rem;
            display: flex; align-items: center; justify-content: center; gap: 15px; text-align: center;
        }
        .filter-box {
            background-color: #f8f9fa; padding: 1.5rem; border-radius: 15px;
            margin-bottom: 2rem; border: 1px solid #edf2f7;
        }
        .btn-filter {
            background: linear-gradient(135deg, #4a90e2, #357abd); border: none;
            border-radius: 10px; padding: 8px 25px; font-weight: 600; transition: 0.3s;
        }
        .table { border-radius: 12px; overflow: hidden; }
        .table thead { background-color: #2c3e50; color: white; }
        .details-container {
            margin-top: 2.5rem; border: none; border-radius: 20px;
            overflow: hidden; box-shadow: 0 15px 40px rgba(0,0,0,0.12) !important;
        }
        .details-header {
            background: linear-gradient(135deg, #2563eb, #1d4ed8);
            color: white; padding: 1.25rem 1.5rem; font-weight: 600;
        }
        .summary-box {
            background: #f8f9fa; padding: 20px; border-radius: 15px;
            border: 1px solid #dee2e6; margin-top: 20px;
        }
    </style>

    <div class="container main-container">
        <div class="page-content-card">
            <h1 class="order-title">
                <i class="fas fa-history text-primary"></i> My Order History
            </h1>

            <%-- Filter Section --%>
            <div class="filter-box">
                <div class="row g-3 align-items-center justify-content-center">
                    <div class="col-auto">
                        <label class="form-label mb-0"><i class="fas fa-filter me-2"></i>Filter by Status:</label>
                    </div>
                    <div class="col-md-4">
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select shadow-sm">
                            <asp:ListItem Text="-- All Orders --" Value="All"></asp:ListItem>
                            <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                            <asp:ListItem Text="Assigned" Value="Assigned"></asp:ListItem>
                            <asp:ListItem Text="Shipped" Value="Shipped"></asp:ListItem>
                            <asp:ListItem Text="Delivered" Value="Delivered"></asp:ListItem>
                            <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary btn-filter shadow-sm" OnClick="btnFilter_Click" />
                    </div>
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="d-block text-center mb-3"></asp:Label>

            <%-- Orders Grid --%>
            <div class="table-responsive">
                <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" 
                    CssClass="table table-hover align-middle" GridLines="None"
                    DataKeyNames="OrderID" OnRowCommand="gvOrders_RowCommand">
                    <HeaderStyle CssClass="table-dark" />
                    <Columns>
                        <asp:TemplateField HeaderText="Order ID">
                            <ItemTemplate><span class="fw-bold">#<%# Eval("OrderID") %></span></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                        <asp:TemplateField HeaderText="Total Amount">
                            <ItemTemplate><span class="fw-bold text-success">Rs. <%# Eval("GrandTotal", "{0:N2}") %></span></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PaymentStatus" HeaderText="Payment" />
                        <asp:BoundField DataField="DeliveryStatus" HeaderText="Delivery Status" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="d-flex gap-2">
                                    <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-outline-primary btn-sm" CommandName="ViewOrder" CommandArgument='<%# Eval("OrderID") %>'><i class="fas fa-eye"></i> Details</asp:LinkButton>
                                    <asp:LinkButton ID="btnInvoice" runat="server" CssClass="btn btn-outline-success btn-sm" CommandName="ViewInvoice" CommandArgument='<%# Eval("OrderID") %>'><i class="fas fa-file-invoice"></i> Invoice</asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <%-- Details Panel --%>
            <asp:Panel ID="pnlOrderDetails" runat="server" Visible="false" CssClass="details-container shadow-lg">
                <div class="details-header d-flex justify-content-between align-items-center">
                    <span><i class="fas fa-info-circle me-2"></i>Order Details - #<asp:Literal ID="litOrderID" runat="server"></asp:Literal></span>
                    <asp:LinkButton ID="btnCloseTop" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseDetails_Click" style="font-size:1.5rem;">&times;</asp:LinkButton>
                </div>
                <div class="card-body p-4 bg-white">
                    <div class="row g-4 mb-4">
                        <div class="col-md-6 border-end">
                            <h6 class="text-primary fw-bold text-uppercase small mb-3">Order Info</h6>
                            <p class="mb-2"><strong>Order ID:</strong> <asp:Label ID="lblOrderID" runat="server" CssClass="badge bg-primary"></asp:Label></p>
                            <p class="mb-2"><strong>Order Date:</strong> <asp:Label ID="lblOrderDate" runat="server" CssClass="text-muted"></asp:Label></p>
                            <p class="mb-2"><strong>Grand Total:</strong> <asp:Label ID="lblGrandTotal" runat="server" Font-Bold="true" ForeColor="#2ecc71" style="font-size: 1.2rem;"></asp:Label></p>
                            <p class="mb-0"><strong>Payment:</strong> <asp:Label ID="lblPaymentStatus" runat="server"></asp:Label></p>
                        </div>
                        <div class="col-md-6 ps-md-4">
                            <h6 class="text-primary fw-bold text-uppercase small mb-3">Delivery Tracker</h6>
                            <p class="mb-2"><strong>Status:</strong> <asp:Label ID="lblDeliveryStatus" runat="server" CssClass="badge bg-info text-dark"></asp:Label></p>
                            <p class="mb-2"><strong>Delivery Person:</strong> <asp:Label ID="lblDeliveryPerson" runat="server" CssClass="text-muted"></asp:Label></p>
                            <p class="mb-2"><strong>Assigned At:</strong> <asp:Label ID="lblAssignedAt" runat="server" CssClass="text-muted small"></asp:Label></p>
                            <p class="mb-0"><strong>Delivered At:</strong> <asp:Label ID="lblDeliveredAt" runat="server" CssClass="text-muted small"></asp:Label></p>
                        </div>
                    </div>

                    <h6 class="mt-4 border-bottom pb-2 fw-bold text-dark"><i class="fas fa-shopping-cart me-2"></i>Purchased Items</h6>
                    
                    <div class="table-responsive mt-3">
                        <asp:GridView ID="gvOrderItems" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm w-100">
                            <HeaderStyle CssClass="table-light text-dark" />
                            <Columns>
                                <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                <asp:BoundField DataField="Quantity" HeaderText="Qty" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Unit Price" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>Rs. <%# Eval("Price", "{0:N2}") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sub Total" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>Rs. <%# (Convert.ToDecimal(Eval("Quantity")) * Convert.ToDecimal(Eval("Price"))).ToString("N2") %></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="row justify-content-end mt-3">
                        <div class="col-md-5 col-lg-4">
                            <div class="summary-box">
                                <div class="d-flex justify-content-between mb-2">
                                    <span class="text-muted">Sub Total:</span>
                                    <span class="fw-bold">Rs. <asp:Label ID="lblSubTotal" runat="server" Text="0.00"></asp:Label></span>
                                </div>
                                <div class="d-flex justify-content-between mb-2">
                                    <span class="text-muted">Tax Amount:</span>
                                    <span class="fw-bold">Rs. <asp:Label ID="lblTaxAmount" runat="server" Text="0.00"></asp:Label></span>
                                </div>
                                <hr />
                                <div class="d-flex justify-content-between">
                                    <span class="h5 fw-bold text-dark">Grand Total:</span>
                                    <span class="h5 fw-bold text-success">Rs. <asp:Label ID="lblGrandTotalSummary" runat="server" Text="0.00"></asp:Label></span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="text-end mt-4">
                        <asp:Button ID="btnCloseDetails" runat="server" Text="Back to List" CssClass="btn btn-secondary px-4 fw-bold rounded-pill" OnClick="btnCloseDetails_Click" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>