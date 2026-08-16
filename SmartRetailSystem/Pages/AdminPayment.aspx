<%@ Page Title="Admin Payment Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPayment.aspx.cs" Inherits="SmartRetailSystem.AdminPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700;800&display=swap" rel="stylesheet">
    <style>
        .admin-payment-container { 
            width: 95%; 
            margin: 20px auto; 
            font-family: 'Poppins', sans-serif;
        }

        .page-header {
            text-align: center;
            margin-bottom: 35px;
        }

        .page-header h2 {
            font-size: 2.8rem;
            font-weight: 800;
            letter-spacing: -1px;
            background: linear-gradient(90deg, #1e293b, #3b82f6);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin: 0;
            display: inline-block;
        }

        .payment-card {
            background: white;
            padding: 30px;
            border-radius: 15px;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
            border: 1px solid #f1f5f9;
        }

        .form-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 20px;
            align-items: flex-end;
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            font-weight: 600;
            color: #475569;
            margin-bottom: 8px;
            font-size: 0.85rem;
            text-transform: uppercase;
        }

        input, select { 
            width: 100%;
            padding: 11px 15px; 
            border-radius: 8px; 
            border: 1px solid #cbd5e1; 
            outline: none;
            transition: 0.3s;
            box-sizing: border-box;
        }

        input:focus, select:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
        }

        .btn-container {
            display: flex;
            gap: 12px;
            margin-top: 10px;
            justify-content: center;
        }

        .btn { 
            padding: 12px 25px; 
            border: none; 
            border-radius: 10px; 
            color: white; 
            cursor: pointer; 
            font-weight: 700;
            transition: all 0.3s ease;
            text-transform: uppercase;
            font-size: 0.8rem;
        }

        .update { background: linear-gradient(135deg, #3b82f6, #2563eb); } 
        .search { background: linear-gradient(135deg, #f59e0b, #d97706); } 
        .delete { background: linear-gradient(135deg, #ef4444, #dc2626); }

        .btn:hover { transform: translateY(-2px); box-shadow: 0 5px 15px rgba(0,0,0,0.1); }

        .grid { 
            background: white;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05);
            border: 1px solid #e2e8f0;
        }

        .grid table { width: 100%; border-collapse: collapse; }
        .grid th { background: #f8fafc; color: #64748b; padding: 15px; font-weight: 700; font-size: 0.75rem; text-transform: uppercase; text-align: center; border-bottom: 2px solid #e2e8f0; }
        .grid td { padding: 15px; border-bottom: 1px solid #f1f5f9; text-align: center; color: #1e293b; font-size: 0.9rem; }

        .select-btn { background: #f1f5f9; color: #475569; border: 1px solid #e2e8f0; border-radius: 6px; padding: 5px 12px; cursor: pointer; font-weight: 600; }
        .select-btn:hover { background: #e2e8f0; }
    </style>

    <div class="admin-payment-container">
        <div class="page-header">
            <h2>💳 Admin Payment Management</h2>
        </div>

        <div class="payment-card">
            <div class="form-row">
                <div class="form-group">
                    <label>Order ID</label>
                    <asp:TextBox ID="txtOrderID" runat="server" placeholder="Ex: 1001" />
                </div>

                <div class="form-group">
                    <label>Amount (Rs.)</label>
                    <asp:TextBox ID="txtAmount" runat="server" placeholder="0.00" />
                </div>

                <div class="form-group">
                    <label>Payment Method</label>
                    <asp:DropDownList ID="ddlMethod" runat="server">
                        <asp:ListItem>Card Payment</asp:ListItem>
                        <asp:ListItem>Bank Transfer</asp:ListItem>
                        <asp:ListItem>Cash On Delivery</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Payment Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Value="Pending">Pending</asp:ListItem>
                        <asp:ListItem Value="Paid">Paid</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Date</label>
                    <asp:TextBox ID="txtDate" runat="server" TextMode="DateTimeLocal" />
                </div>
            </div>

            <div class="btn-container">
                <asp:Button ID="btnUpdate" runat="server" Text="🔄 Update Status & Payment" CssClass="btn update" OnClick="btnUpdate_Click"/>
                <asp:Button ID="btnSearch" runat="server" Text="🔍 Search Order" CssClass="btn search" OnClick="btnSearch_Click"/>
                <asp:Button ID="btnDelete" runat="server" Text="❌ Delete Record" CssClass="btn delete" OnClick="btnDelete_Click" OnClientClick="return confirm('Warning: This will delete the payment record. Continue?');"/>
            </div>
        </div>

        <div class="grid">
            <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="false" DataKeyNames="PaymentID" 
                OnSelectedIndexChanged="gvPayments_SelectedIndexChanged" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="PaymentID" HeaderText="Pay ID"/>
                    <asp:BoundField DataField="OrderID" HeaderText="Order ID"/>
                    <asp:BoundField DataField="PaidAmount" HeaderText="Amount" DataFormatString="Rs. {0:N2}"/>
                    <asp:BoundField DataField="PaymentMethod" HeaderText="Method"/>
                    <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}"/>
                    <asp:BoundField DataField="PaymentStatus" HeaderText="Status"/>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnSelect" runat="server" Text="Select" CssClass="select-btn" CommandName="Select" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        
        <asp:Label ID="lblMessage" runat="server" style="display:block; margin-top:15px; text-align:center; font-weight:600;" />
    </div>
</asp:Content>