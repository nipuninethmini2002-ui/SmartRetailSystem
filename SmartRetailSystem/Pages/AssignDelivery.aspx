<%@ Page Title="Assign Delivery" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignDelivery.aspx.cs" Inherits="SmartRetailSystem.AssignDelivery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .assign-delivery-wrapper { 
            font-family: 'Poppins', 'Segoe UI', Arial, sans-serif; 
            background: #f8fafc; 
            padding: 40px 20px; 
            min-height: 100vh;
        }

        .content-card {
            max-width: 1000px;
            margin: 0 auto;
            background: white;
            padding: 40px;
            border-radius: 20px;
            box-shadow: 0 10px 25px -5px rgba(0,0,0,0.05);
            border: 1px solid #f1f5f9;
        }

        /* --- නවීන Gradient Font Style එක සහිත මාතෘකාව --- */
        .page-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-header h1 { 
            font-family: 'Poppins', 'Segoe UI', sans-serif;
            font-size: 3rem;
            font-weight: 800;
            letter-spacing: -1px;
            text-transform: capitalize;
            /* Blue-Indigo Gradient Effect */
            background: linear-gradient(90deg, #1e3a8a, #3b82f6);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin: 0;
            display: inline-block;
        }

        /* Form Design */
        .form-section {
            display: grid;
            grid-template-columns: 1fr 1.5fr auto;
            gap: 20px;
            align-items: flex-end;
            background: #f8fafc;
            padding: 30px;
            border-radius: 15px;
            border: 1px dashed #cbd5e1;
            margin-bottom: 40px;
        }

        .input-group {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .input-group label {
            font-weight: 700;
            color: #475569;
            font-size: 0.85rem;
            text-transform: uppercase;
        }

        .custom-input {
            padding: 12px 15px;
            border: 1px solid #cbd5e1;
            border-radius: 10px;
            font-size: 1rem;
            transition: all 0.3s;
            outline: none;
        }

        .custom-input:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
        }

        .btn-assign { 
            padding: 12px 25px; 
            background: linear-gradient(135deg, #1e3a8a, #3b82f6); 
            color: white; 
            border: none; 
            border-radius: 10px; 
            font-weight: 700;
            cursor: pointer; 
            transition: all 0.3s;
            box-shadow: 0 4px 6px rgba(37, 99, 235, 0.2);
        }

        .btn-assign:hover { 
            transform: translateY(-2px);
            box-shadow: 0 6px 12px rgba(37, 99, 235, 0.3);
        }

        .success-msg { 
            color: #059669; 
            font-weight: bold; 
            text-align: center;
            display: block;
            margin-top: 15px;
        }

        /* GridView Styling */
        .table-title {
            color: #1e293b;
            font-size: 1.4rem;
            font-weight: 700;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .custom-grid { 
            width: 100%; 
            border-collapse: collapse; 
            margin-top: 10px;
            border-radius: 12px;
            overflow: hidden;
            border: none;
        }

        .custom-grid th { 
            background: #f1f5f9; 
            color: #475569; 
            padding: 18px; 
            text-transform: uppercase;
            font-size: 0.8rem;
            font-weight: 700;
            border-bottom: 2px solid #e2e8f0;
            text-align: center;
        }

        .custom-grid td { 
            border-bottom: 1px solid #f1f5f9; 
            padding: 15px; 
            text-align: center; 
            color: #1e293b;
            font-size: 0.95rem;
        }

        /* Status Badges */
        .status-badge {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.75rem;
            font-weight: 700;
            text-transform: uppercase;
        }

        .status-paid { background: #dcfce7; color: #15803d; }
        .status-delivery { background: #eff6ff; color: #1d4ed8; }

        .custom-grid tr:hover { background: #fcfdfe; }
    </style>

    <div class="assign-delivery-wrapper">
        <div class="content-card">
            
            <div class="page-header">
                <h1>🚚 Assign Delivery</h1>
            </div>

            <div class="form-section">
                <div class="input-group">
                    <label>Order ID</label>
                    <asp:TextBox ID="txtOrderID" runat="server" CssClass="custom-input" placeholder="Ex: 1001"></asp:TextBox>
                </div>

                <div class="input-group">
                    <label>Delivery Person</label>
                    <asp:DropDownList ID="ddlDeliveryPerson" runat="server" CssClass="custom-input"></asp:DropDownList>
                </div>

                <asp:Button ID="btnAssign" runat="server" Text="Confirm Assignment" CssClass="btn-assign" OnClick="btnAssign_Click" />
            </div>

            <asp:Label ID="lblMsg" runat="server" CssClass="success-msg"></asp:Label>

            <div style="margin-top: 50px;">
                <h3 class="table-title">📦 Pending Paid Orders</h3>
                
                <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" CssClass="custom-grid" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="OrderID" HeaderText="Order ID" ItemStyle-Font-Bold="true" />
                        <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                        <asp:BoundField DataField="GrandTotal" HeaderText="Amount" DataFormatString="Rs. {0:N2}" ItemStyle-ForeColor="#1e293b" />
                        
                        <asp:TemplateField HeaderText="Payment">
                            <ItemTemplate>
                                <span class="status-badge status-paid">
                                    <%# Eval("PaymentStatus") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Delivery">
                            <ItemTemplate>
                                <span class="status-badge status-delivery">
                                    <%# Eval("DeliveryStatus") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="text-align:center; padding: 40px; color: #94a3b8; font-style: italic;">
                            No pending orders found for delivery.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>