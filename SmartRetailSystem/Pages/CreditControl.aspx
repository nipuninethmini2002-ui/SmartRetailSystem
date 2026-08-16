<%@ Page Title="Credit Control" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreditControl.aspx.cs" Inherits="SmartRetailSystem.CreditControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .credit-wrapper {
            max-width: 1200px;
            margin: 40px auto;
            font-family: 'Poppins', sans-serif;
            padding: 0 20px;
        }

        .page-header-centered { text-align: center; margin-bottom: 50px; }
        .page-header-centered h1 {
            font-size: 3.2rem;
            font-weight: 800;
            background: linear-gradient(90deg, #1e293b, #4f46e5);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin: 0;
        }

        .system-status {
            background: #f0fdf4;
            color: #166534;
            padding: 8px 20px;
            border-radius: 30px;
            font-size: 0.9rem;
            font-weight: 700;
            display: inline-block;
            border: 1px solid #bbf7d0;
        }

        .content-card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.04);
            padding: 30px;
            margin-bottom: 40px;
            border: 1px solid #f1f5f9;
        }

        /* Table Styles */
        .modern-grid { width: 100%; border-collapse: collapse; }
        .modern-grid th { 
            background: #f8fafc; 
            color: #475569; 
            padding: 18px 15px; 
            text-align: left; 
            font-size: 0.8rem; 
            text-transform: uppercase; 
            border-bottom: 2px solid #f1f5f9; 
        }
        .modern-grid td { padding: 18px 15px; border-bottom: 1px solid #f1f5f9; color: #1e293b; }

        /* Status Badges - Bootstrap නැතත් වැඩ කරන ලෙස */
        .status-badge {
            padding: 6px 12px;
            border-radius: 8px;
            font-size: 0.75rem;
            font-weight: 800;
            text-transform: uppercase;
            color: white;
            display: inline-block;
        }
        .status-active { background-color: #10b981; } /* Green */
        .status-blocked { background-color: #ef4444; } /* Red */

        .warning-label { 
            color: #dc2626; 
            background: #fef2f2; 
            padding: 6px 12px; 
            border-radius: 8px; 
            font-size: 0.75rem; 
            font-weight: 700;
        }

        /* Buttons */
        .btn-custom { padding: 10px 16px; border-radius: 10px; font-weight: 700; border: none; cursor: pointer; font-size: 0.8rem; text-transform: uppercase; }
        .btn-block { background: #ef4444; color: white; }
        .btn-remind { background: #f59e0b; color: white; }
        .btn-unblock { background: #10b981; color: white; }

        /* Notifications */
        .notification-unread { border-left: 6px solid #f59e0b; background: #fffbeb; padding: 15px; border-radius: 10px; margin-bottom: 10px; }
        .notification-read { border-left: 6px solid #cbd5e1; background: #f8fafc; padding: 15px; border-radius: 10px; margin-bottom: 10px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="credit-wrapper">
        <div class="page-header-centered">
            <h1>📊 Credit Control Dashboard</h1>
            <div class="system-status">🛡️ Real-time Credit Monitoring Active</div>
        </div>

        <div class="content-card">
            <asp:GridView ID="gvCreditControl" runat="server" AutoGenerateColumns="false" 
                CssClass="modern-grid" OnRowCommand="gvCreditControl_RowCommand" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="CustomerID" HeaderText="ID" ItemStyle-Width="50px" />
                    <asp:BoundField DataField="Name" HeaderText="Customer Name" ItemStyle-Font-Bold="true" />
                    <asp:BoundField DataField="CreditLimit" HeaderText="Limit" DataFormatString="Rs. {0:N0}" />
                    <asp:BoundField DataField="OutstandingBalance" HeaderText="Outstanding" DataFormatString="Rs. {0:N2}" ItemStyle-Font-Bold="true" />
                    <asp:BoundField DataField="OverdueDays" HeaderText="Overdue" ItemStyle-HorizontalAlign="Center" />
                    
                    <asp:TemplateField HeaderText="Current Status">
                        <ItemTemplate>
                            <span class='status-badge <%# Eval("Status").ToString() == "Blocked" ? "status-blocked" : "status-active" %>'>
                                <%# Eval("Status").ToString() == "" ? "Active" : Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Management">
                        <ItemTemplate>
                            <div style="display: flex; flex-direction: column; gap: 8px;">
                                <div style="display: flex; gap: 5px;">
                                    <asp:Button ID="btnBlock" runat="server" Text="🚫 Block" CommandName="Block" CommandArgument='<%# Eval("CustomerID") %>' 
                                        CssClass="btn-custom btn-block" Visible='<%# Eval("Status").ToString() != "Blocked" %>' />
                                    
                                    <asp:Button ID="btnUnblock" runat="server" Text="✅ Unblock" CommandName="Unblock" CommandArgument='<%# Eval("CustomerID") %>' 
                                        CssClass="btn-custom btn-unblock" Visible='<%# Eval("Status").ToString() == "Blocked" %>' />

                                    <asp:Button ID="btnReminder" runat="server" Text="✉️ Remind" CommandName="Reminder" CommandArgument='<%# Eval("CustomerID") %>' 
                                        CssClass="btn-custom btn-remind" />
                                </div>
                                <asp:PlaceHolder runat="server" Visible='<%# Convert.ToInt32(Eval("OverdueDays")) > 30 %>'>
                                    <span class="warning-label">⚠️ High Risk: Over 30 Days</span>
                                </asp:PlaceHolder>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="notif-section">
            <h4 style="margin-bottom:20px;">⚡ Recent Alerts</h4>
            <asp:GridView ID="gvNotifications" runat="server" AutoGenerateColumns="false" CssClass="modern-grid" ShowHeader="false" GridLines="None">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class='<%# Convert.ToBoolean(Eval("IsRead")) ? "notification-read" : "notification-unread" %>'>
                                <div style="display: flex; justify-content: space-between;">
                                    <strong><%# Eval("CustomerName") %></strong>
                                    <small><%# Eval("CreatedAt", "{0:MMM dd, HH:mm}") %></small>
                                </div>
                                <p style="margin: 5px 0 0 0; font-size: 0.9rem;"><%# Eval("Message") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>