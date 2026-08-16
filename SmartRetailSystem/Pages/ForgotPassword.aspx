<%@ Page Title="Forgot Password | Smart Retail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="SmartRetailSystem.ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f4f7f6;
        }

        /* Forgot Password Container Styling */
        .forgot-wrapper {
            max-width: 500px;
            margin: 60px auto;
            background-color: #ffffff;
            padding: 40px;
            border-radius: 20px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            border: 1px solid #eee;
        }

        /* Centered H1 Title */
        .forgot-title {
            text-align: center;
            color: #2c3e50;
            font-weight: 700;
            margin-bottom: 15px;
            font-size: 2.2rem;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
        }

        .instruction-text {
            text-align: center;
            color: #64748b;
            font-size: 0.95rem;
            margin-bottom: 30px;
            line-height: 1.6;
        }

        .form-label {
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 10px;
            display: block;
        }

        .form-control-custom {
            width: 100%;
            padding: 12px 15px;
            border-radius: 12px;
            border: 1px solid #cbd5e1;
            background-color: #f8fafc;
            transition: all 0.3s ease;
            font-family: 'Poppins', sans-serif;
        }

        .form-control-custom:focus {
            border-color: #2563eb;
            outline: none;
            background-color: #fff;
            box-shadow: 0 0 0 4px rgba(37, 99, 235, 0.1);
        }

        .btn-action {
            width: 100%;
            padding: 12px;
            border-radius: 12px;
            font-weight: 600;
            margin-top: 20px;
            transition: all 0.3s;
            border: none;
            font-family: 'Poppins', sans-serif;
            cursor: pointer;
        }

        .btn-send { 
            background: linear-gradient(135deg, #2563eb, #1d4ed8); 
            color: white; 
        }
        
        .btn-send:hover { 
            transform: translateY(-2px); 
            box-shadow: 0 5px 15px rgba(37, 99, 235, 0.3);
        }

        .btn-reset { 
            background: linear-gradient(135deg, #10b981, #059669); 
            color: white; 
        }
        
        .btn-reset:hover { 
            transform: translateY(-2px); 
            box-shadow: 0 5px 15px rgba(16, 185, 129, 0.3);
        }

        .btn-back { 
            background-color: #f1f5f9; 
            color: #475569; 
            margin-top: 15px; 
        }
        
        .btn-back:hover { 
            background-color: #e2e8f0; 
            color: #1e293b;
        }

        .message-label {
            display: block;
            text-align: center;
            margin-top: 20px;
            font-weight: 500;
        }

        /* Validation Styling */
        .text-danger {
            font-size: 0.85rem;
            margin-top: 5px;
            display: block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="forgot-wrapper">
        <h1 class="forgot-title">
            <i class="fas fa-key text-primary"></i> Password Recovery
        </h1>
        <p class="instruction-text">Enter your registered email address to verify your account and set a new password.</p>

        <div class="mb-4">
            <asp:Label ID="lblEmail" runat="server" CssClass="form-label" Text="Email Address"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control-custom" Placeholder="e.g. name@company.com"></asp:TextBox>
            
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="<i class='fas fa-exclamation-circle'></i> Email is required" 
                CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                ErrorMessage="<i class='fas fa-info-circle'></i> Enter a valid email address" 
                CssClass="text-danger" Display="Dynamic"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"></asp:RegularExpressionValidator>
        </div>

        <asp:Button ID="btnSend" runat="server" Text="Verify Email" CssClass="btn-action btn-send shadow-sm" OnClick="btnSend_Click" />

        <%-- Reset Password Panel (Initially Hidden) --%>
        <asp:Panel ID="pnlReset" runat="server" Visible="false" CssClass="mt-4 pt-4 border-top">
            <h5 class="text-center fw-bold mb-4 text-dark">Set New Password</h5>
            
            <div class="mb-3">
                <asp:Label ID="lblNewPassword" runat="server" Text="New Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control-custom" TextMode="Password" placeholder="••••••••"></asp:TextBox>
            </div>

            <div class="mb-4">
                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm New Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control-custom" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                
                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ControlToCompare="txtNewPassword" ErrorMessage="<i class='fas fa-times-circle'></i> Passwords do not match" 
                    CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
            </div>

            <asp:Button ID="btnReset" runat="server" Text="Update Password" CssClass="btn-action btn-reset shadow-sm" OnClick="btnReset_Click" />
        </asp:Panel>

        <asp:Button ID="btnBack" runat="server" Text="Back to Login" CssClass="btn-action btn-back" OnClick="btnBack_Click" CausesValidation="false" />

        <div class="message-label">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>
    </div>
</asp:Content>