<%@ Page Title="Product Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductList.aspx.cs" Inherits="SmartRetailSystem.Pages.ProductList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .inventory-wrapper {
            padding: 30px;
            background-color: #f1f5f9;
            border-radius: 15px;
            min-height: 80vh;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        /* --- නවීන Gradient Font Style එක සහිත මාතෘකාව --- */
        .header-container {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-title {
            font-family: 'Poppins', 'Segoe UI', sans-serif;
            font-size: 3rem;
            font-weight: 800;
            letter-spacing: -1px;
            text-transform: capitalize;
            /* Gradient Effect */
            background: linear-gradient(90deg, #0f172a, #2563eb);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin-bottom: 15px;
            display: block;
        }

        /* Search Section Card */
        .search-card {
            background: white;
            padding: 25px;
            border-radius: 15px;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
            max-width: 800px;
            margin-left: auto;
            margin-right: auto;
        }

        .custom-input-group {
            display: flex;
            gap: 15px;
            align-items: center;
        }

        .search-box {
            flex: 1;
            height: 48px !important;
            border-radius: 10px !important;
            border: 1px solid #cbd5e1 !important;
            padding: 10px 18px !important;
            font-size: 1rem;
            transition: 0.3s;
        }

        .search-box:focus {
            border-color: #2563eb !important;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
            outline: none;
        }

        /* Button Styling */
        .btn-action {
            height: 48px !important;
            padding: 0 28px !important;
            border-radius: 10px !important;
            font-weight: 600 !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s ease;
            text-decoration: none !important;
            border: none !important;
            cursor: pointer;
        }

        .btn-add { background: linear-gradient(135deg, #2563eb, #1d4ed8) !important; color: white !important; margin-top: 10px; }
        .btn-add:hover { transform: translateY(-3px); box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3); }

        .btn-search { background-color: #334155 !important; color: white !important; }
        .btn-search:hover { background-color: #1e293b !important; }

        /* GridView Styling */
        .table-container {
            background: white;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
        }

        .modern-table {
            width: 100%;
            border-collapse: collapse;
        }

        .modern-table th {
            background-color: #f8fafc !important;
            color: #475569 !important;
            padding: 18px 15px !important;
            text-align: left;
            font-weight: 700;
            text-transform: uppercase;
            font-size: 0.85rem;
            border-bottom: 2px solid #e2e8f0;
        }

        .modern-table td {
            padding: 18px 15px !important;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
            font-size: 0.95rem;
        }

        .low-stock-warning {
            color: #ef4444;
            font-weight: 700;
            background: #fef2f2;
            padding: 4px 10px;
            border-radius: 6px;
            border: 1px solid #fee2e2;
        }

        .grid-btn {
            padding: 8px 15px !important;
            font-size: 0.85rem !important;
            border-radius: 8px !important;
            margin-right: 5px;
            height: auto !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="inventory-wrapper">
        
        <div class="header-container">
            <h1 class="page-title">📦 Product Inventory</h1>
            <asp:LinkButton ID="btnAddNewProduct" runat="server" OnClick="btnAddNewProduct_Click" CssClass="btn-action btn-add">
                <i class="fas fa-plus me-2"></i> Add New Product
            </asp:LinkButton>
        </div>

        <asp:Panel ID="pnlLowStock" runat="server" Visible="false" CssClass="mb-4" style="background: #fff5f5; border-left: 5px solid #ef4444; padding: 20px; border-radius: 12px; max-width: 800px; margin: 0 auto 30px auto; box-shadow: 0 2px 4px rgba(0,0,0,0.05);">
            <div style="color: #991b1b; font-weight: bold; font-size: 1.1rem; margin-bottom: 5px;">
                <i class="fas fa-exclamation-triangle me-2"></i> Critical Low Stock Alert
            </div>
            <asp:Label ID="lblLowStockAlert" runat="server" style="color: #b91c1c;"></asp:Label>
        </asp:Panel>

        <div class="search-card">
            <div class="custom-input-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" Placeholder="Search by product name, ID or category..."></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="🔍 Search" OnClick="btnSearch_Click" CssClass="btn-action btn-search" />
            </div>
        </div>

        <div class="table-container">
            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="ProductID" OnRowCommand="gvProducts_RowCommand" 
                CssClass="modern-table" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="ProductID" HeaderText="ID" ItemStyle-Width="80px" />
                    <asp:BoundField DataField="ProductName" HeaderText="Product Name" ItemStyle-Font-Bold="true" />
                    
                    <asp:TemplateField HeaderText="Category">
                        <ItemTemplate>
                            <span style="background: #f1f5f9; color: #475569; padding: 5px 12px; border-radius: 20px; font-size: 0.8rem; font-weight: 600; border: 1px solid #e2e8f0;">
                                <%# Eval("Category") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Price" HeaderText="Unit Price" DataFormatString="Rs. {0:N2}" ItemStyle-ForeColor="#0f172a" />
                    
                    <asp:TemplateField HeaderText="Current Stock">
                        <ItemTemplate>
                            <span class='<%# (Eval("StockQuantity") != DBNull.Value && Convert.ToInt32(Eval("StockQuantity")) < 10) ? "low-stock-warning" : "" %>'>
                                <i class="fas fa-boxes me-1"></i> <%# Eval("StockQuantity") != DBNull.Value ? Eval("StockQuantity") : "0" %> Units
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Status" HeaderText="Status" />

                    <asp:TemplateField HeaderText="Management">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditProduct" 
                                CommandArgument='<%# Eval("ProductID") %>' CssClass="btn-action grid-btn" style="background: #10b981; color: white;" />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteProduct" 
                                CommandArgument='<%# Eval("ProductID") %>' CssClass="btn-action grid-btn" style="background: #ef4444; color: white;"
                                OnClientClick="return confirm('Are you sure you want to delete this product?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="padding: 60px; text-align: center; color: #94a3b8;">
                        <i class="fas fa-search-minus fa-4x mb-3" style="opacity: 0.3;"></i>
                        <p style="font-size: 1.2rem;">No products match your search criteria.</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>