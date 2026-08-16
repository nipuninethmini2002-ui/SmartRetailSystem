<%@ Page Title="Customer Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerDashboard.aspx.cs" Inherits="SmartRetailSystem.CustomerDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f4f7f6;
        }

        .container-fluid { width: 90%; margin: 30px auto; }
        
        .dashboard-title { 
            text-align: center; 
            color: #2c3e50; 
            margin-bottom: 30px; 
            font-weight: 700;
            letter-spacing: -0.5px;
        }

        /* Top Action Buttons */
        .top-buttons { 
            display: flex; 
            flex-wrap: wrap; 
            gap: 15px; 
            justify-content: center; 
            margin-bottom: 40px; 
        }

        .btn-custom { 
            padding: 12px 25px; 
            border: none; 
            border-radius: 10px; 
            font-size: 0.95em; 
            color: #fff; 
            cursor: pointer; 
            transition: all 0.3s ease; 
            text-decoration: none; 
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            min-width: 170px; 
            font-weight: 600;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05);
        }

        .btn-main { background: linear-gradient(135deg, #4a90e2, #357abd); }
        .btn-info-custom { background: linear-gradient(135deg, #17a2b8, #138496); }
        .btn-logout { background: linear-gradient(135deg, #e74c3c, #c0392b); }

        .btn-custom:hover { 
            transform: translateY(-3px); 
            box-shadow: 0 6px 12px rgba(0,0,0,0.15); 
            color: white;
            opacity: 1;
        }

        /* Stats Cards */
        .cards { 
            display: grid; 
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); 
            gap: 20px; 
            margin-bottom: 40px;
        }

        .card-custom { 
            background: #fff; 
            padding: 30px; 
            border-radius: 20px; 
            text-align: center; 
            transition: all 0.3s ease; 
            border: 1px solid #eee;
            position: relative;
            overflow: hidden;
        }

        /* විශේෂ වර්ණ ගැන්වීම් */
        .card-total { border-bottom: 5px solid #4a90e2; }
        .card-pending { border-bottom: 5px solid #f1c40f; }
        .card-delivered { border-bottom: 5px solid #2ecc71; }
        .card-balance { border-bottom: 5px solid #e74c3c; }

        .card-custom:hover { 
            transform: translateY(-8px);
            box-shadow: 0 12px 25px rgba(0,0,0,0.08);
        }

        .card-custom h2 { 
            font-size: 2.5em; 
            margin: 0; 
            font-weight: 700; 
            color: #2c3e50;
        }

        .card-custom p { 
            margin: 10px 0 0 0; 
            font-size: 1em; 
            font-weight: 500; 
            color: #7f8c8d;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        /* Notifications Section */
        .notifications-container { 
            background: #ffffff; 
            padding: 30px; 
            border-radius: 20px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.05); 
            border: 1px solid #eee;
        }
        
        .notifications-header { 
            display: flex; 
            align-items: center; 
            justify-content: space-between;
            margin-bottom: 25px; 
            padding-bottom: 15px;
            border-bottom: 2px solid #f8f9fa;
        }

        .notifications-header h3 {
            font-size: 1.3rem;
            font-weight: 600;
            color: #2c3e50;
            margin: 0;
        }

        .notif-item { 
            display: flex; 
            justify-content: space-between; 
            align-items: center; 
            padding: 18px 25px; 
            margin-bottom: 15px; 
            border-radius: 15px; 
            transition: all 0.2s;
            border: 1px solid transparent;
        }

        .notif-unread { 
            background-color: #fffaf0; 
            border: 1px solid #f1c40f44;
            border-left: 6px solid #f1c40f;
        }

        .notif-read { 
            background-color: #fdfdfd; 
            border: 1px solid #eee;
            border-left: 6px solid #bdc3c7;
            opacity: 0.8;
        }

        .notif-content { flex: 1; padding-right: 15px; }
        .notif-msg { font-size: 1rem; color: #2d3436; font-weight: 500; display: block; margin-bottom: 5px; }
        .notif-time { font-size: 0.85rem; color: #95a5a6; display: flex; align-items: center; gap: 5px; }

        .btn-mark-read {
            background-color: #2ecc71;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 50px;
            font-size: 0.85rem;
            font-weight: 600;
            cursor: pointer;
            transition: 0.3s;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-mark-read:hover {
            background-color: #27ae60;
            color: white;
            box-shadow: 0 4px 10px rgba(46, 204, 113, 0.3);
        }

        .empty-notif {
            text-align: center;
            padding: 40px;
            color: #95a5a6;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <h1 class="dashboard-title">
            <i class="fas fa-user-circle" style="color: #4a90e2; margin-right: 10px;"></i>
            Welcome, <asp:Label ID="lblCustomerName" runat="server" Text="Customer"></asp:Label>
        </h1>

        <div class="cards">
            <div class="card-custom card-total">
                <p>Total Orders</p>
                <h2><asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label></h2>
            </div>
            <div class="card-custom card-pending">
                <p>Pending Orders</p>
                <h2><asp:Label ID="lblPendingOrders" runat="server" Text="0"></asp:Label></h2>
            </div>
            <div class="card-custom card-delivered">
                <p>Delivered Orders</p>
                <h2><asp:Label ID="lblDeliveredOrders" runat="server" Text="0"></asp:Label></h2>
            </div>
            <div class="card-custom card-balance">
                <p>Outstanding (Rs.)</p>
                <h2><asp:Label ID="lblOutstandingBalance" runat="server" Text="0.00"></asp:Label></h2>
            </div>
        </div>

        <div class="notifications-container">
            <div class="notifications-header">
                <h3><i class="fas fa-bell" style="color: #f1c40f;"></i> Recent Notifications</h3>
            </div>

            <asp:GridView ID="gvNotifications" runat="server" AutoGenerateColumns="false" 
                OnRowCommand="gvNotifications_RowCommand" CssClass="table table-borderless" GridLines="None" ShowHeader="false">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class='notif-item <%# Convert.ToBoolean(Eval("IsRead")) ? "notif-read" : "notif-unread" %>'>
                                <div class="notif-content">
                                    <span class="notif-msg">
                                        <%# Convert.ToBoolean(Eval("IsRead")) ? "" : "<i class='fas fa-circle' style='color:#f1c40f; font-size:10px; margin-right:10px;'></i>" %>
                                        <%# Eval("Message") %>
                                    </span>
                                    <span class="notif-time">
                                        <i class="far fa-clock"></i> <%# Eval("CreatedAt", "{0:MMM dd, yyyy - hh:mm tt}") %>
                                    </span>
                                </div>
                                
                                <asp:LinkButton ID="lnkMarkRead" runat="server" CommandName="MarkRead" 
                                    CommandArgument='<%# Eval("NotificationID") %>' 
                                    Visible='<%# !Convert.ToBoolean(Eval("IsRead")) %>'
                                    CssClass="btn-mark-read">
                                    <i class="fas fa-check"></i> Read
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="empty-notif">
                        <i class="fas fa-bell-slash fa-3x mb-3" style="opacity: 0.3;"></i><br />
                        <span>You don't have any notifications right now.</span>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>