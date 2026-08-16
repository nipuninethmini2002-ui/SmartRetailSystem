<%@ Page Title="Staff Dashboard - Smart Retail System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StaffDashboard.aspx.cs" Inherits="SmartRetailSystem.StaffDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* මුළු Dashboard එකේම පසුබිම සහ Font පාලනය */
        .dashboard-wrapper {
            background-color: #f4f7f6;
            min-height: 90vh;
            padding: 40px 20px;
            font-family: 'Poppins', sans-serif;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        /* Heading Style */
        .dashboard-header {
            text-align: center;
            margin-bottom: 50px;
        }

        .dashboard-header h1 {
            color: #333;
            font-weight: 700;
            font-size: 32px;
            margin-bottom: 10px;
        }

        .dashboard-header p {
            color: #777;
            font-size: 16px;
        }

        /* Cards Layout */
        .cards-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 25px;
            padding: 10px;
        }

        /* Individual Card Style */
        .stat-card {
            background: #ffffff;
            padding: 30px;
            border-radius: 15px;
            text-align: center;
            box-shadow: 0 10px 20px rgba(0,0,0,0.05);
            border-bottom: 5px solid #007bff; /* කලින් Blue theme එකට ගැලපීමට */
            transition: all 0.3s ease;
        }

        .stat-card:hover {
            transform: translateY(-10px);
            box-shadow: 0 15px 30px rgba(0,0,0,0.1);
        }

        /* Card එක ඇතුළත Numbers */
        .stat-card h2 {
            font-size: 36px;
            font-weight: 700;
            color: #007bff;
            margin: 0;
            margin-bottom: 10px;
        }

        /* Card එක ඇතුළත Labels */
        .stat-card p {
            color: #555;
            font-size: 15px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin: 0;
        }

        /* වෙනස් වර්ණ සහිත Borders (විකල්ප) */
        .card-products { border-bottom-color: #007bff; }
        .card-orders { border-bottom-color: #28a745; }
        .card-pending { border-bottom-color: #ffc107; }
        .card-customers { border-bottom-color: #17a2b8; }
        .card-credit { border-bottom-color: #e74c3c; }
        .card-stock { border-bottom-color: #6f42c1; }

        /* Responsive */
        @media (max-width: 768px) {
            .dashboard-header h1 { font-size: 26px; }
            .stat-card h2 { font-size: 30px; }
        }
    </style>

    <div class="dashboard-wrapper">
        <div class="container">
            <div class="dashboard-header">
                <h1>Staff Dashboard</h1>
                <p>Welcome back! Here is an overview of your retail system.</p>
            </div>

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
                    <h2 style="font-size: 28px;">Rs. <asp:Label ID="lblOutstandingCredit" runat="server" Text="0.00"></asp:Label></h2>
                    <p>Outstanding Credit</p>
                </div>

                <div class="stat-card card-stock">
                    <h2 style="color: #e74c3c;"><asp:Label ID="lblLowStock" runat="server" Text="0"></asp:Label></h2>
                    <p>Low Stock Items</p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>