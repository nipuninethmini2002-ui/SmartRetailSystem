<%@ Page Title="Admin Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="SmartRetailSystem.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700;800&family=Inter:wght@400;600&display=swap" rel="stylesheet">
    
    <style type="text/css">
        .main-container {
            min-height: 100vh;
            padding: 20px;
            background-color: #f8fafc;
            font-family: 'Inter', sans-serif;
            overflow-x: hidden;
        }

        /* --- ✅ කලින් පිටුවේ තිබූ එම නිල් පැහැති Gradient Header එක --- */
        .page-header-container {
            text-align: center;
            margin-bottom: 40px;
            padding-top: 10px;
        }

        .page-header-container h1 {
            font-family: 'Poppins', sans-serif;
            font-size: 3rem;
            font-weight: 800;
            letter-spacing: -1px;
            text-transform: capitalize;
            /* ඔබ ඉල්ලූ නිල් පැහැති Gradient එක */
            background: linear-gradient(90deg, #1e293b, #3b82f6);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin: 0;
            display: inline-block;
        }

        /* Card Layout */
        .report-card {
            background: #ffffff;
            border-radius: 15px;
            border: 1px solid #eef2f6;
            box-shadow: 0 10px 25px rgba(0,0,0,0.03);
            padding: 30px;
            margin-bottom: 30px;
        }

        .section-title {
            font-size: 1rem;
            font-weight: 700;
            color: #475569;
            margin-bottom: 25px;
            display: flex;
            align-items: center;
            gap: 10px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        /* Filter Grid */
        .filter-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
        }

        .input-box label {
            font-weight: 600;
            color: #64748b;
            margin-bottom: 8px;
            display: block;
            font-size: 0.85rem;
        }

        .custom-input {
            width: 100%;
            border-radius: 8px;
            border: 1px solid #cbd5e1;
            padding: 10px 15px;
            font-size: 0.9rem;
            transition: all 0.3s;
            background-color: #fff;
            color: #334155;
        }

        .custom-input:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
            outline: none;
        }

        /* Buttons Styling */
        .button-group {
            display: flex;
            justify-content: center;
            gap: 15px;
            margin-top: 35px;
            flex-wrap: wrap;
        }

        .btn-report {
            padding: 12px 28px;
            font-weight: 700;
            border-radius: 10px;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
            color: white;
            font-size: 0.85rem;
            display: flex;
            align-items: center;
            gap: 8px;
            text-transform: uppercase;
        }

        .btn-gen { background: #3b82f6; box-shadow: 0 4px 10px rgba(59, 130, 246, 0.2); }
        .btn-gen:hover { background: #2563eb; transform: translateY(-2px); }

        .btn-pdf { background: #ef4444; box-shadow: 0 4px 10px rgba(239, 68, 68, 0.2); }
        .btn-pdf:hover { background: #dc2626; transform: translateY(-2px); }

        .btn-xls { background: #10b981; box-shadow: 0 4px 10px rgba(16, 185, 129, 0.2); }
        .btn-xls:hover { background: #059669; transform: translateY(-2px); }

        /* GridView Table Styling */
        .table-container {
            overflow-x: auto;
            border-radius: 12px;
            border: 1px solid #f1f5f9;
        }

        .modern-gv {
            width: 100% !important;
            border-collapse: collapse;
            border: none !important;
        }

        .modern-gv th {
            background-color: #f8fafc !important;
            color: #475569 !important;
            padding: 15px !important;
            font-weight: 700 !important;
            font-size: 0.8rem !important;
            text-transform: uppercase;
            border-bottom: 2px solid #e2e8f0 !important;
            text-align: center !important;
        }

        .modern-gv td {
            padding: 14px !important;
            border-bottom: 1px solid #f1f5f9 !important;
            color: #1e293b !important;
            font-size: 0.9rem !important;
            text-align: center !important;
        }

        /* ✅ Error එක නිවැරදි කර සකස් කළ Empty Row Style එක */
        .empty-row-style {
            text-align: center;
            padding: 40px !important;
            color: #94a3b8 !important;
            font-style: italic;
            font-size: 1.1rem;
        }

        .modern-gv tr:hover {
            background-color: #fcfdfe !important;
        }

        /* Clean Junk */
        p, .&nbsp; { display: none; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-container">
        
        <div class="page-header-container">
            <h1>📊 Reports & Analytics</h1>
        </div>

        <div class="container" style="max-width: 1100px; margin: 0 auto;">
            
            <div class="report-card">
                <div class="section-title">🔍 Filter Your Report</div>
                <div class="filter-grid">
                    <div class="input-box">
                        <label>From Date</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="custom-input" TextMode="Date" />
                    </div>
                    <div class="input-box">
                        <label>To Date</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="custom-input" TextMode="Date" />
                    </div>
                    <div class="input-box">
                        <label>Customer</label>
                        <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="custom-input">
                            <asp:ListItem Text="-- All Customers --" Value="0" />
                        </asp:DropDownList>
                    </div>
                    <div class="input-box">
                        <label>Category</label>
                        <asp:DropDownList ID="ddlProduct" runat="server" CssClass="custom-input">
                            <asp:ListItem Text="-- All Products --" Value="0" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="button-group">
                    <asp:Button ID="btnGenerate" runat="server" Text="📊 Generate" CssClass="btn-report btn-gen" OnClick="btnGenerate_Click" />
                    <asp:Button ID="btnExportPDF" runat="server" Text="📄 PDF Export" CssClass="btn-report btn-pdf" OnClick="btnExportPDF_Click" />
                    <asp:Button ID="btnExportExcel" runat="server" Text="📈 Excel Export" CssClass="btn-report btn-xls" OnClick="btnExportExcel_Click" />
                </div>
            </div>

            <div class="report-card">
                <div class="section-title">📋 Data Overview</div>
                <div class="table-container">
                    <asp:GridView ID="gvReports" runat="server" 
                        CssClass="modern-gv" 
                        AutoGenerateColumns="true"
                        GridLines="None"
                        EmptyDataText="No records found for the selected date range.">
                        <HeaderStyle CssClass="gv-header" />
                        <%-- දෝෂය ඇති වූ 'style' property එක මෙතැනින් ඉවත් කර ඇත --%>
                        <EmptyDataRowStyle CssClass="empty-row-style" />
                    </asp:GridView>
                </div>
            </div>
            
        </div>
    </div>
</asp:Content>