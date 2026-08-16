<%@ Page Title="Invoice | Smart Retail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="SmartRetailSystem.Invoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --primary-color: #2563eb;
            --secondary-color: #64748b;
            --dark-color: #0f172a;
            --success-color: #16a34a;
            --border-color: #e2e8f0;
            --light-bg: #f8fafc;
        }

        .invoice-container { margin-top: 30px; margin-bottom: 50px; }

        .invoice-card {
            background: #ffffff;
            padding: 50px;
            border-radius: 16px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.05);
            border: 1px solid var(--border-color);
            position: relative;
        }

        /* Top Accent Bar */
        .invoice-card::before {
            content: ""; position: absolute; top: 0; left: 0; right: 0; height: 8px;
            background: linear-gradient(90deg, var(--primary-color), #7c3aed);
            border-radius: 16px 16px 0 0;
        }

        /* Header Styling */
        .invoice-header {
            display: flex; justify-content: space-between; align-items: flex-start;
            border-bottom: 2px solid var(--light-bg);
            padding-bottom: 30px; margin-bottom: 30px;
        }

        .company-name { font-size: 1.8rem; font-weight: 800; color: var(--primary-color); margin-bottom: 5px; }
        .invoice-title { font-size: 2.2rem; font-weight: 900; color: var(--dark-color); margin-bottom: 0; }
        .invoice-meta { font-weight: 700; color: var(--primary-color); font-size: 1.1rem; }

        /* Information Sections */
        .section-title {
            font-size: 0.85rem; font-weight: 800; color: var(--secondary-color);
            text-transform: uppercase; letter-spacing: 1px;
            margin-bottom: 12px; border-bottom: 1px solid var(--border-color);
            padding-bottom: 5px;
        }

        .info-group p { margin-bottom: 6px; font-size: 0.95rem; }
        .info-label { font-weight: 600; color: var(--secondary-color); width: 100px; display: inline-block; }
        .info-value { color: var(--dark-color); font-weight: 500; }

        /* Table/Grid Styling */
        .grid-container { margin-top: 30px; }
        .custom-grid { width: 100% !important; border: none !important; margin-bottom: 0; }
        .custom-grid th {
            background-color: var(--light-bg) !important;
            color: var(--secondary-color) !important;
            padding: 12px 15px !important;
            font-weight: 700 !important;
            text-transform: uppercase; font-size: 0.8rem;
            border-bottom: 2px solid var(--border-color) !important;
        }
        .custom-grid td { padding: 15px !important; border-bottom: 1px solid var(--light-bg) !important; font-size: 0.95rem; }

        /* Totals Area */
        .summary-wrapper { display: flex; justify-content: flex-end; margin-top: 30px; }
        .summary-box {
            background-color: var(--light-bg);
            padding: 25px; border-radius: 12px; width: 100%; max-width: 350px;
        }
        .summary-row { display: flex; justify-content: space-between; margin-bottom: 10px; font-size: 1rem; }
        .summary-row.total { 
            margin-top: 15px; padding-top: 15px; border-top: 1px dashed var(--border-color);
            font-weight: 800; font-size: 1.2rem; color: var(--dark-color);
        }

        /* Buttons */
        .action-buttons { margin-top: 40px; display: flex; gap: 15px; justify-content: center; }
        .btn-action {
            padding: 12px 28px; font-weight: 700; border-radius: 8px; 
            transition: all 0.2s ease; border: none; cursor: pointer;
            display: flex; align-items: center; gap: 8px;
        }
        .btn-print { background-color: var(--dark-color); color: white; }
        .btn-pdf { background-color: var(--primary-color); color: white; }
        .btn-back { background-color: #f1f5f9; color: var(--secondary-color); text-decoration: none; }
        
        .btn-action:hover { transform: translateY(-2px); opacity: 0.9; color: white; }
        .btn-back:hover { background-color: var(--border-color); color: var(--dark-color); }

        .status-badge { font-weight: 700; font-size: 0.8rem; }

        @media print {
            .no-print, nav, footer, .action-buttons { display: none !important; }
            .invoice-card { box-shadow: none; border: none; padding: 0; }
            .invoice-card::before { display: none; }
            body { background: white; }
        }
    </style>
    <script type="text/javascript">
        function printInvoice() { window.print(); }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container invoice-container">
        <div class="invoice-card">
            
            <%-- 1. Header Section --%>
            <div class="invoice-header">
                <div>
                    <div class="company-name">SMART RETAIL</div>
                    <p class="text-muted small mb-0">
                        123 Business Hub, Colombo 01, Sri Lanka<br />
                        Hotline: +94 112 345 678 | support@smartretail.com
                    </p>
                </div>
                <div class="text-end">
                    <h1 class="invoice-title">INVOICE</h1>
                    <div class="invoice-meta">#<asp:Label ID="lblInvoiceNumber" runat="server" Text="INV-0000"></asp:Label></div>
                    <div class="text-muted small">Date: <asp:Label ID="lblOrderDate" runat="server"></asp:Label></div>
                </div>
            </div>

            <%-- 2. Information Section --%>
            <div class="row mb-4">
                <div class="col-md-6 info-group">
                    <div class="section-title">Bill To</div>
                    <p><span class="info-label">Customer</span> <asp:Label ID="lblCustomerName" runat="server" CssClass="info-value fw-bold"></asp:Label></p>
                    <p><span class="info-label">Address</span> <asp:Label ID="lblShippingAddress" runat="server" CssClass="info-value"></asp:Label></p>
                    <p><span class="info-label">Contact</span> <asp:Label ID="lblContact" runat="server" CssClass="info-value"></asp:Label></p>
                </div>
                <div class="col-md-6 info-group">
                    <div class="section-title">Payment Info</div>
                    <p><span class="info-label">Method</span> <asp:Label ID="lblPaymentMethod" runat="server" CssClass="info-value"></asp:Label></p>
                    <p>
                        <span class="info-label">Status</span> 
                        <asp:Label ID="lblStatusBadge" runat="server" CssClass="badge status-badge" Text="Pending"></asp:Label>
                    </p>
                </div>
            </div>

            <%-- 3. Items Grid --%>
            <div class="grid-container">
                <div class="section-title">Order Items</div>
                <asp:GridView ID="gvInvoiceDetails" runat="server" AutoGenerateColumns="False" 
                    CssClass="table custom-grid" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="ProductName" HeaderText="Item Description" />
                        <asp:BoundField DataField="Quantity" HeaderText="Qty" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Price" HeaderText="Unit Price" DataFormatString="Rs. {0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="Rs. {0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>
            </div>

            <%-- 4. Summary Section --%>
            <div class="summary-wrapper">
                <div class="summary-box">
                    <div class="summary-row">
                        <span class="text-secondary">Subtotal</span>
                        <span class="fw-bold">Rs. <asp:Label ID="lblTotalAmount" runat="server" Text="0.00"></asp:Label></span>
                    </div>
                    <div class="summary-row text-success">
                        <span class="fw-bold">Amount Paid</span>
                        <span class="fw-bold">Rs. <asp:Label ID="lblPaidAmount" runat="server" Text="0.00"></asp:Label></span>
                    </div>
                    <div class="summary-row total">
                        <span>Balance Due</span>
                        <span class="text-danger">Rs. <asp:Label ID="lblBalance" runat="server" Text="0.00"></asp:Label></span>
                    </div>
                </div>
            </div>

            <%-- 5. Footer --%>
            <div class="mt-5 pt-4 border-top text-center text-muted">
                <p class="small mb-1">Thank you for choosing <strong>Smart Retail System</strong> for your purchase!</p>
                <p class="small" style="font-size: 0.75rem;">This is a computer-generated invoice. No signature required.</p>
            </div>

            <%-- 6. Actions (No Print) --%>
            <div class="action-buttons no-print">
                <a href="CustomerOrders.aspx" class="btn-action btn-back">
                    <i class="fas fa-arrow-left"></i> My Orders
                </a>
                <button type="button" onclick="printInvoice()" class="btn-action btn-print">
                    <i class="fas fa-print"></i> Print Invoice
                </button>
                <asp:LinkButton ID="btnDownloadPDF" runat="server" CssClass="btn-action btn-pdf text-decoration-none" OnClick="btnDownloadPDF_Click">
                    <i class="fas fa-file-pdf"></i> Save as PDF
                </asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Content>