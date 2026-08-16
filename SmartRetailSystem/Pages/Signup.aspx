<%@ Page Title="Sign Up - Smart Integrated Retail System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="SmartRetailSystem.Signup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        /* මුළු පිටුවේම පෙනුම Login පිටුවට සමාන කිරීමට */
        .signup-wrapper {
            background-color: #f4f7f6;
            min-height: 90vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Poppins', sans-serif;
            padding: 40px 20px;
        }

        .signup-container {
            max-width: 500px;
            width: 100%;
            background: #ffffff;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.1);
            border-top: 5px solid #28a745; /* Signup සඳහා කොළ පැහැය භාවිතා කර ඇත */
        }

        /* Heading Style */
        .signup-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .signup-header h2 {
            color: #333;
            font-weight: 700;
            margin-bottom: 5px;
            font-size: 28px;
        }

        .signup-header p {
            color: #777;
            font-size: 14px;
        }

        /* Form Grouping */
        .form-group {
            margin-bottom: 18px;
        }

        .form-label {
            font-weight: 600;
            color: #444;
            display: block;
            margin-bottom: 8px;
            font-size: 14px;
        }

        .form-control-custom {
            width: 100%;
            padding: 10px 15px;
            border: 1px solid #ddd;
            border-radius: 8px;
            transition: all 0.3s ease;
            box-sizing: border-box;
            font-family: inherit;
        }

        .form-control-custom:focus {
            border-color: #28a745;
            box-shadow: 0 0 8px rgba(40, 167, 69, 0.25);
            outline: none;
        }

        /* Button Styles */
        .btn-signup {
            width: 100%;
            background: linear-gradient(135deg, #28a745, #218838);
            color: white;
            padding: 12px;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            font-size: 16px;
            cursor: pointer;
            transition: transform 0.2s;
            margin-top: 10px;
        }

        .btn-signup:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(40, 167, 69, 0.3);
        }

        /* Message Styles */
        .message {
            display: block;
            text-align: center;
            color: #dc3545;
            margin-top: 15px;
            font-weight: 500;
            font-size: 14px;
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

        /* Responsive Fix */
        @media (max-width: 480px) {
            .signup-container {
                padding: 25px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="signup-wrapper">
        <div class="signup-container">
            <div class="signup-header">
                <h2>Create Account</h2>
                <p>Join Smart Integrated Retail System</p>
            </div>

            <div class="form-group">
                <asp:Label ID="lblName" runat="server" Text="Full Name" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-custom" Placeholder="Enter your full name"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblEmail" runat="server" Text="Email Address" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control-custom" TextMode="Email" Placeholder="example@mail.com"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblUsername" runat="server" Text="Username" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control-custom" Placeholder="Choose a username"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server" Text="Password" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control-custom" TextMode="Password" Placeholder="Create a password"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblRole" runat="server" Text="Select Your Role" CssClass="form-label"></asp:Label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control-custom" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                    <asp:ListItem Text="Staff" Value="Staff" />
                    <asp:ListItem Text="Customer" Value="Customer" />
                    <asp:ListItem Text="Delivery" Value="Delivery" />
                </asp:DropDownList>
            </div>

            <asp:Panel ID="customerFields" runat="server" Visible="false">
                <div class="form-group">
                    <asp:Label ID="lblPhone" runat="server" Text="Phone Number" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control-custom" Placeholder="07X XXX XXXX"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblAddress" runat="server" Text="Delivery Address" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control-custom" Placeholder="Enter your permanent address"></asp:TextBox>
                </div>
            </asp:Panel>

            <asp:Button ID="btnSignup" runat="server" Text="Sign Up" CssClass="btn-signup" OnClick="btnSignup_Click" />
            
            <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>

            <div class="link-footer">
                Already have an account? <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="Login.aspx">Login</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>