<%@ Page Title="Login - Smart Integrated Retail System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SmartRetailSystem.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* මුළු පිටුවේම පෙනුම පාලනය කිරීමට */
        .login-wrapper {
            background-color: #f4f7f6;
            min-height: 85vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Poppins', sans-serif; /* කලින් පාවිච්චි කළ Style එක */
            padding: 20px;
        }

        .login-container {
            max-width: 450px;
            width: 100%;
            background: #ffffff;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.1);
            border-top: 5px solid #007bff;
        }

        /* Heading Style එක - Dashboard එකට සමානයි */
        .login-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .login-header h2 {
            color: #333;
            font-weight: 700;
            margin-bottom: 5px;
            font-size: 28px;
        }

        .login-header p {
            color: #777;
            font-size: 14px;
        }

        /* Form පාලනය */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            font-weight: 600;
            color: #444;
            display: block;
            margin-bottom: 8px;
        }

        .form-control-custom {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #ddd;
            border-radius: 8px;
            transition: all 0.3s ease;
            box-sizing: border-box;
        }

        .form-control-custom:focus {
            border-color: #007bff;
            box-shadow: 0 0 8px rgba(0,123,255,0.25);
            outline: none;
        }

        /* Button Styles */
        .btn-login {
            width: 100%;
            background: linear-gradient(135deg, #007bff, #0056b3);
            color: white;
            padding: 12px;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 16px;
            cursor: pointer;
            transition: transform 0.2s;
            margin-bottom: 10px;
        }

        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0,123,255,0.3);
        }

        .btn-reset {
            width: 100%;
            background: #f8f9fa;
            color: #666;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-weight: 500;
            cursor: pointer;
        }

        .btn-reset:hover {
            background: #e2e6ea;
        }

        .error-msg {
            color: #dc3545;
            text-align: center;
            font-size: 14px;
            margin-top: 15px;
            font-weight: 500;
            display: block;
        }

        .link-footer {
            text-align: center;
            margin-top: 25px;
            font-size: 14px;
            color: #666;
        }

        .link-footer a {
            color: #007bff;
            text-decoration: none;
            font-weight: 600;
        }

        .link-footer a:hover {
            text-decoration: underline;
        }

        /* Validation Style */
        .text-danger {
            font-size: 12px;
            color: #dc3545;
            margin-top: 5px;
            display: block;
        }
    </style>

    <div class="login-wrapper">
        <div class="login-container">
            <div class="login-header">
                <h2>Login</h2>
                <p>Smart Integrated Retail System</p>
            </div>

            <div class="form-group">
                <asp:Label ID="lblUsername" runat="server" CssClass="form-label" Text="Username"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control-custom" Placeholder="Enter your username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server" CssClass="form-label" Text="Password"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control-custom" TextMode="Password" Placeholder="Enter your password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-reset" OnClick="btnReset_Click" CausesValidation="false" />

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-msg"></asp:Label>

            <div class="link-footer">
                <asp:HyperLink ID="lnkForgotPassword" runat="server" NavigateUrl="ForgotPassword.aspx">Forgot Password?</asp:HyperLink>
                <div style="margin-top: 10px;">
                    Don't have an account? <asp:HyperLink ID="lnkSignup" runat="server" NavigateUrl="Signup.aspx">Sign Up</asp:HyperLink>
                </div>
            </div>
        </div>
    </div>
</asp:Content>