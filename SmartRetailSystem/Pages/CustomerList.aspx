<%@ Page Title="Customer List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerList.aspx.cs" Inherits="SmartRetailSystem.CustomerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .list-container {
            width: 100%;
            margin: 20px auto;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        /* --- නවීන Gradient Font Style එක සහිත මාතෘකාව --- */
        .page-header-container {
            text-align: center;
            margin-bottom: 35px;
        }

        .page-header-container h1 {
            font-family: 'Poppins', 'Segoe UI', sans-serif;
            font-size: 3rem;
            font-weight: 800;
            letter-spacing: -1px;
            text-transform: capitalize;
            /* Gradient Effect */
            background: linear-gradient(90deg, #1e293b, #3b82f6);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin-bottom: 10px;
            display: inline-block;
        }

        /* ✅ MODERN TOP BAR */
        .top-bar {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 25px;
            background: #fff;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);
            border: 1px solid #f1f5f9;
        }

        .search-group {
            display: flex;
            gap: 10px;
        }

        .search-box {
            padding: 10px 15px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            font-size: 0.95rem;
            transition: 0.3s;
            outline: none;
        }

        .search-box:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
        }

        /* BUTTON STYLES */
        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            color: white;
            cursor: pointer;
            font-weight: 600;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s ease;
            font-size: 0.9rem;
        }

        .add-btn { background: linear-gradient(135deg, #3b82f6, #2563eb); }
        .edit-btn { background: #10b981; }
        .delete-btn { background: #ef4444; }

        .btn:hover { 
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            opacity: 0.9;
            color: white;
        }

        /* GRIDVIEW TABLE STYLE */
        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            background: white;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 10px 15px -3px rgba(0,0,0,0.05);
            border: none;
        }

        .custom-grid th {
            background: #f8fafc;
            color: #475569;
            padding: 18px;
            text-transform: uppercase;
            font-size: 0.8rem;
            font-weight: 700;
            border-bottom: 2px solid #e2e8f0;
            text-align: center;
        }

        .custom-grid td {
            padding: 16px;
            text-align: center;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
            font-size: 0.95rem;
        }

        .custom-grid tr:hover {
            background-color: #fcfdfe;
        }

        /* Balance highlight */
        .outstanding {
            color: #e11d48;
            font-weight: 700;
        }
    </style>

    <div class="list-container">
        
        <div class="page-header-container">
            <h1>👥 Customer Management</h1>
        </div>

        <div class="top-bar">
            <div class="search-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" Placeholder="Search by Name or ID..." Width="280px" />
                <asp:Button ID="btnSearch" runat="server" Text="🔍 Search" CssClass="btn add-btn" OnClick="btnSearch_Click" />
            </div>

            <div class="right-actions">
                <asp:LinkButton ID="btnAddCustomer" runat="server" CssClass="btn add-btn" OnClick="btnAddCustomer_Click">
                    <i class="fas fa-user-plus mr-2"></i> &nbsp; Add New Customer
                </asp:LinkButton>
            </div>
        </div>

        <asp:GridView ID="gvCustomers" runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="CustomerID" 
            CssClass="custom-grid"
            OnRowCommand="gvCustomers_RowCommand"
            GridLines="None">
            
            <Columns>
                <asp:BoundField DataField="CustomerID" HeaderText="ID" ItemStyle-Font-Bold="true" />
                <asp:BoundField DataField="Name" HeaderText="Customer Name" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                
                <asp:TemplateField HeaderText="Credit (Rs.)">
                    <ItemTemplate>
                        <%# Eval("CreditLimit", "{0:N2}") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Balance">
                    <ItemTemplate>
                        <span class='<%# Convert.ToDecimal(Eval("OutstandingBalance")) > 0 ? "outstanding" : "" %>'>
                            <%# Eval("OutstandingBalance", "{0:N2}") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn edit-btn" 
                            CommandName="EditCustomer" CommandArgument='<%# Eval("CustomerID") %>' style="padding: 6px 14px; font-size: 0.8rem;" />
                        &nbsp;
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn delete-btn" 
                            CommandName="DeleteCustomer" CommandArgument='<%# Eval("CustomerID") %>' 
                            OnClientClick="return confirm('Are you sure you want to delete this customer?');" style="padding: 6px 14px; font-size: 0.8rem;" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div style="text-align:center; padding:40px; color: #94a3b8;">
                    <p style="font-size: 1.1rem;">No customer records found.</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>