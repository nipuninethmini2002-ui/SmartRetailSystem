<%@ Page Title="Admin Dashboard - Smart Retail System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SmartRetailSystem.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* මුළු Dashboard එකේම පසුබිම සහ Font පාලනය */
        .dashboard-wrapper {
            background-color: #f4f7f6;
            min-height: 90vh;
            padding: 40px 20px;
            font-family: 'Poppins', 'Segoe UI', sans-serif;
        }

        .container-fluid {
            max-width: 1200px;
            margin: 0 auto;
        }

        /* Heading Style - Staff Dashboard එකට සමානයි */
        .dashboard-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .dashboard-header h1 {
            color: #333;
            font-weight: 700;
            font-size: 36px;
            margin-bottom: 10px;
            letter-spacing: -1px;
        }

        .dashboard-header p {
            color: #777;
            font-size: 16px;
        }

        /* Alert Container Style */
        .alert-container {
            background-color: #fff3cd;
            color: #856404;
            padding: 15px 25px;
            border-radius: 12px;
            border-left: 5px solid #ffc107;
            margin-bottom: 35px;
            text-align: center;
            font-weight: 500;
            box-shadow: 0 4px 10px rgba(0,0,0,0.05);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Cards Layout */
        .cards-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 25px;
            padding: 10px;
        }

        /* Individual Card Style (Staff Dashboard style) */
        .stat-card {
            background: #ffffff;
            padding: 35px 25px;
            border-radius: 15px;
            text-align: center;
            box-shadow: 0 8px 20px rgba(0,0,0,0.05);
            transition: all 0.3s ease;
            border-bottom: 5px solid #007bff;
        }

        .stat-card:hover {
            transform: translateY(-10px);
            box-shadow: 0 12px 25px rgba(0,0,0,0.1);
        }

        /* Card Numbers */
        .stat-card h2 {
            font-size: 38px;
            font-weight: 700;
            color: #333;
            margin: 0 0 10px 0;
            letter-spacing: -1px;
        }

        /* Card Labels */
        .stat-card p {
            color: #666;
            font-size: 14px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin: 0;
        }

        /* Admin Dashboard එකට ගැලපෙන Color Coding */
        .card-products { border-bottom-color: #007bff; }  /* Blue */
        .card-orders { border-bottom-color: #28a745; }    /* Green */
        .card-pending { border-bottom-color: #17a2b8; }   /* Cyan */
        .card-customers { border-bottom-color: #6f42c1; } /* Purple */
        .card-credit { border-bottom-color: #fd7e14; }    /* Orange */
        .card-lowstock { border-bottom-color: #dc3545; }  /* Red */

        /* Number Colors for emphasis */
        .card-products h2 { color: #007bff; }
        .card-orders h2 { color: #28a745; }
        .card-lowstock h2 { color: #dc3545; }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-wrapper">
        <div class="container-fluid">
            
            <div class="dashboard-header">
                <h1>Admin Dashboard</h1>
                <p>System-wide overview and management console</p>
            </div>

            <asp:Panel ID="pnlLowStockAlert" runat="server" Visible="false" CssClass="alert-container">
                <span>⚠️ <strong>Attention:</strong> Some items are running low on stock.</span>
                <asp:LinkButton ID="lnkViewLowStock" runat="server" OnClick="btnManageProducts_Click" 
                    style="color: #856404; text-decoration: underline; margin-left: 10px; font-weight: 700;">
                    Review Inventory
                </asp:LinkButton>
            </asp:Panel>

            <div class="cards-grid">
                <div class="stat-card card-products">
                    <h2><asp:Label ID="lblTotalProducts" runat="server" Text="0"></asp:Label></h2>
                    <p>Total Products</p>
                </div>
                
                <div class="stat-card card-orders">
                    <h2><asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label></h2>
                    <p>Total Orders</p>
                </div>
                
                <div class="stat-card card-pending">
                    <h2><asp:Label ID="lblPendingOrders" runat="server" Text="0"></asp:Label></h2>
                    <p>Pending Orders</p>
                </div>
                
                <div class="stat-card card-customers">
                    <h2><asp:Label ID="lblTotalCustomers" runat="server" Text="0"></asp:Label></h2>
                    <p>Total Customers</p>
                </div>
                
                <div class="stat-card card-credit">
                    <h2 style="font-size: 30px;">Rs. <asp:Label ID="lblOutstandingCredit" runat="server" Text="0.00"></asp:Label></h2>
                    <p>Outstanding Credit</p>
                </div>
                
                <div class="stat-card card-lowstock">
                    <h2><asp:Label ID="lblLowStock" runat="server" Text="0"></asp:Label></h2>
                    <p>Low Stock Items</p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>