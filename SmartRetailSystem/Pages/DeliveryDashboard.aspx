<%@ Page Title="Delivery Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeliveryDashboard.aspx.cs" Inherits="SmartRetailSystem.DeliveryDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f4f7f6;
        }

        .main-container { 
            padding: 2rem 3% !important;
        }

        .page-content-card {
            background: #ffffff;
            border-radius: 20px;
            padding: 2.5rem;
            box-shadow: 0 10px 30px rgba(0,0,0,0.05);
            border: 1px solid #eee;
            margin-top: 20px;
        }

        /* Centered H1 Title Styling */
        .dashboard-title {
            font-size: 2.2rem;
            font-weight: 700;
            color: #2c3e50;
            margin-bottom: 2rem;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
            text-align: center;
        }

        /* Button Styling */
        .btn-custom {
            border-radius: 10px;
            font-weight: 600;
            padding: 8px 20px;
            transition: 0.3s;
        }

        .btn-logout {
            background-color: #e74c3c;
            color: white;
            border: none;
        }

        .btn-logout:hover {
            background-color: #c0392b;
            color: white;
            transform: translateY(-2px);
        }

        .btn-update {
            background: linear-gradient(135deg, #4a90e2, #357abd);
            color: white;
            border: none;
        }

        .btn-update:hover {
            box-shadow: 0 4px 12px rgba(74, 144, 226, 0.3);
            color: white;
        }

        /* GridView Styling */
        .table {
            border-radius: 12px;
            overflow: hidden;
            border: none !important;
        }

        .table thead {
            background-color: #2c3e50;
            color: white;
        }

        .table thead th {
            font-weight: 500;
            padding: 15px;
            border: none;
        }

        .table tbody td {
            padding: 15px;
            vertical-align: middle;
            color: #4a5568;
            border-bottom: 1px solid #edf2f7;
        }

        /* Status Dropdown Styling */
        .status-select {
            border-radius: 8px;
            border: 1px solid #cbd5e0;
            padding: 5px 10px;
            font-size: 0.9rem;
            outline: none;
        }

        .msg-label {
            display: block;
            text-align: center;
            font-weight: 600;
            margin-bottom: 15px;
        }
    </style>

    <div class="container-fluid main-container">
        <div class="page-content-card">

            <h1 class="dashboard-title">
                <i class="fas fa-truck-loading text-primary"></i> Delivery Dashboard
            </h1>
            
            <asp:Label ID="lblMessage" runat="server" CssClass="msg-label"></asp:Label>

            <%-- Deliveries Grid --%>
            <div class="table-responsive">
                <asp:GridView ID="gvDeliveries" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="DeliveryID" OnRowDataBound="gvDeliveries_RowDataBound"
                    CssClass="table table-hover align-middle" GridLines="None">
                    <HeaderStyle CssClass="table-dark" />
                    <Columns>
                        <asp:TemplateField HeaderText="Order ID">
                            <ItemTemplate>
                                <span class="fw-bold text-dark">#<%# Eval("OrderID") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                        
                        <asp:TemplateField HeaderText="Address">
                            <ItemTemplate>
                                <span class="small text-muted"><%# Eval("Address") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Update Status">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select status-select shadow-sm">
                                    <asp:ListItem>Pending</asp:ListItem>
                                    <asp:ListItem>Packed</asp:ListItem>
                                    <asp:ListItem>Shipped</asp:ListItem>
                                    <asp:ListItem>Out for Delivery</asp:ListItem>
                                    <asp:ListItem>Delivered</asp:ListItem>
                                    <asp:ListItem>Cancelled</asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="AssignedAt" HeaderText="Assigned Time" DataFormatString="{0:MMM dd, HH:mm}" />
                        
                        <asp:TemplateField HeaderText="Delivered At">
                            <ItemTemplate>
                                <%# Eval("DeliveredAt") == DBNull.Value || Eval("DeliveredAt") == null ? 
                                    "<span class='badge bg-light text-secondary border'>Waiting...</span>" : 
                                    Convert.ToDateTime(Eval("DeliveredAt")).ToString("MMM dd, HH:mm") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="btnUpdate" runat="server" CssClass="btn btn-custom btn-update shadow-sm w-100"
                                    CommandArgument='<%# Eval("DeliveryID") %>' OnClick="btnUpdate_Click">
                                    <i class="fas fa-save me-1"></i> Update
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center p-5 text-muted">
                            <i class="fas fa-shipping-fast fa-3x mb-3"></i><br />
                            <h5>No active deliveries assigned to you.</h5>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>