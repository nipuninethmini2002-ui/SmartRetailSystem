<%@ Page Title="Add Customer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddCustomer.aspx.cs" Inherits="SmartRetailSystem.AddCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700;800&family=Inter:wght@400;600&display=swap" rel="stylesheet">
    
    <style>
        .main-wrapper {
            min-height: 80vh;
            padding: 20px;
            font-family: 'Inter', sans-serif;
            background-color: #f8fafc;
        }

        /* --- ✅ එම නිල් පැහැති Gradient Header එක --- */
        .page-header-container {
            text-align: center;
            margin-bottom: 30px;
        }

        .page-header-container h1 {
            font-family: 'Poppins', sans-serif;
            font-size: 2.8rem;
            font-weight: 800;
            letter-spacing: -1px;
            background: linear-gradient(90deg, #1e293b, #3b82f6);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin-bottom: 5px;
            display: inline-block;
        }

        /* Form Container */
        .add-cust-container { 
            max-width: 600px; 
            margin: 0 auto; 
            background: #fff; 
            padding: 35px; 
            border-radius: 15px; 
            box-shadow: 0 10px 25px rgba(0,0,0,0.05);
            border: 1px solid #f1f5f9;
        }

        label { 
            display: block; 
            margin-top: 15px; 
            font-weight: 600; 
            color: #475569;
            font-size: 0.9rem;
            margin-bottom: 5px;
        }

        .form-control { 
            width: 100%; 
            padding: 12px 15px; 
            border-radius: 8px; 
            border: 1px solid #cbd5e1; 
            box-sizing: border-box;
            font-size: 0.95rem;
            transition: all 0.3s ease;
            outline: none;
            color: #1e293b;
        }

        .form-control:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
        }

        /* Buttons Styling */
        .button-group {
            margin-top: 30px;
            display: flex;
            gap: 12px;
        }

        .btn { 
            flex: 1;
            padding: 12px; 
            border: none; 
            border-radius: 10px; 
            font-weight: 700;
            cursor: pointer; 
            transition: all 0.3s;
            font-size: 0.9rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .btn-save { 
            background: linear-gradient(135deg, #3b82f6, #2563eb); 
            color: #fff; 
            box-shadow: 0 4px 12px rgba(59, 130, 246, 0.2);
        }

        .btn-save:hover { 
            transform: translateY(-2px);
            opacity: 0.95; 
        }

        .btn-cancel { 
            background: #f1f5f9; 
            color: #64748b; 
        }

        .btn-cancel:hover { 
            background: #e2e8f0; 
        }

        .msg-label {
            display: block; 
            margin-top: 15px; 
            font-size: 0.85rem;
            text-align: center;
            font-weight: 600;
        }
    </style>

    <div class="main-wrapper">
        <div class="page-header-container">
            <h1>👤 Add New Customer</h1>
        </div>

        <div class="add-cust-container">
            <div style="border-bottom: 2px solid #f1f5f9; margin-bottom: 20px; padding-bottom: 10px;">
                <span style="color: #94a3b8; font-size: 0.8rem; font-weight: bold; text-transform: uppercase;">Customer Profile Details</span>
            </div>

            <label>Full Name *</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter customer's name" />

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                <div>
                    <label>Phone Number *</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="e.g. 0771234567" />
                </div>
                <div>
                    <label>Credit Limit (Rs.)</label>
                    <asp:TextBox ID="txtCreditLimit" runat="server" Text="50000" CssClass="form-control" />
                </div>
            </div>

            <label>Address *</label>
            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" placeholder="Residential address" />

            <label>Email Address *</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="email@example.com" />

            <div style="background: #f8fafc; padding: 15px; border-radius: 10px; margin-top: 20px; border: 1px dashed #cbd5e1;">
                <span style="color: #64748b; font-size: 0.75rem; font-weight: 800; text-transform: uppercase;">Login Credentials</span>
                
                <label style="margin-top: 10px;">Username *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />

                <label>Password *</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <div class="button-group">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-save" Text="Save Customer" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-cancel" Text="Cancel" OnClick="btnCancel_Click" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="msg-label" ForeColor="#ef4444" />
        </div>
    </div>
</asp:Content>