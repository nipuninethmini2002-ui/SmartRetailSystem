<%@ Page Title="Edit Customer | Smart Retail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditCustomer.aspx.cs" Inherits="SmartRetailSystem.EditCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    
    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f4f7f6;
        }

        .edit-container { 
            max-width: 650px; 
            margin: 50px auto; 
            background: #fff; 
            padding: 40px; 
            border-radius: 20px; 
            box-shadow: 0 15px 35px rgba(0,0,0,0.08);
            border: 1px solid #eee;
        }
        
        /* Centered H1 Title */
        .edit-container h1 { 
            text-align: center; 
            color: #2c3e50; 
            margin-bottom: 30px; 
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 12px;
            font-size: 2.2rem;
        }

        .form-group { margin-bottom: 1.5rem; }
        
        label { 
            display: block; 
            margin-bottom: 10px; 
            font-weight: 600; 
            color: #4a5568;
            font-size: 0.95rem;
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

        .button-group { 
            display: flex; 
            gap: 15px; 
            margin-top: 40px; 
        }

        .btn-action {
            padding: 12px; 
            border: none; 
            border-radius: 12px; 
            font-weight: 600; 
            transition: all 0.3s; 
            font-family: 'Poppins', sans-serif;
            cursor: pointer;
        }

        .btn-save { 
            flex: 2; 
            color: #fff; 
            background: linear-gradient(135deg, #2563eb, #1d4ed8); 
        }

        .btn-save:hover { 
            transform: translateY(-2px); 
            box-shadow: 0 5px 15px rgba(37, 99, 235, 0.3);
        }

        .btn-cancel { 
            flex: 1; 
            color: #4a5568; 
            background-color: #e2e8f0; 
            text-align: center;
            text-decoration: none;
        }

        .btn-cancel:hover { 
            background-color: #cbd5e1; 
            color: #2d3748;
        }

        /* Icon styling */
        .input-icon-group {
            position: relative;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="edit-container">
        <h1><i class="fas fa-user-edit text-primary"></i> Edit Customer</h1>

        <div class="form-group">
            <label for="txtName"><i class="fas fa-user me-2"></i>Full Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control-custom" placeholder="Enter customer name" />
        </div>

        <div class="form-group">
            <label for="txtPhone"><i class="fas fa-phone me-2"></i>Phone Number</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control-custom" placeholder="e.g. 0712345678" />
        </div>

        <div class="form-group">
            <label for="txtAddress"><i class="fas fa-map-marker-alt me-2"></i>Home Address</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control-custom" placeholder="Enter home address" />
        </div>

        <div class="form-group">
            <label for="txtEmail"><i class="fas fa-envelope me-2"></i>Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control-custom" TextMode="Email" placeholder="example@mail.com" />
        </div>

        <div class="row">
            <div class="col-md-6">
                <div class="form-group">
                    <label for="txtCreditLimit"><i class="fas fa-wallet me-2"></i>Credit Limit (Rs.)</label>
                    <asp:TextBox ID="txtCreditLimit" runat="server" CssClass="form-control-custom" TextMode="Number" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-group">
                    <label for="ddlStatus"><i class="fas fa-info-circle me-2"></i>Account Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control-custom">
                        <asp:ListItem Text="Active" Value="Active" />
                        <asp:ListItem Text="Blocked" Value="Blocked" />
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="button-group">
            <asp:Button ID="btnSave" runat="server" CssClass="btn-action btn-save shadow-sm" Text="Save Changes" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="btn-action btn-cancel shadow-sm" Text="Cancel" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>