<%@ Page Title="Payment - Smart Retail System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="SmartRetailSystem.Payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Site.Master එකේ බලපෑම ඉවත් කර මුල් Design එක ආරක්ෂා කිරීමට */
        .payment-page-scoped {
            all: revert;
            display: block;
            font-family: Arial, sans-serif;
            background-color: #f0f8f5;
            padding: 40px 20px;
            min-height: 80vh;
        }

        .payment-container { 
            max-width: 550px; 
            margin: 0 auto; 
            padding: 30px; 
            background-color: #fff; 
            border-radius: 12px; 
            box-shadow: 0 5px 20px rgba(0,0,0,0.15); 
        }

        .payment-container h2 { 
            text-align: center; 
            color: #2E7D32; 
            margin-bottom: 25px; 
            margin-top: 0;
            font-weight: bold;
        }

        .payment-container label { 
            display: block; 
            margin-top: 15px; 
            font-weight: bold; 
            color: #333;
        }

        .payment-container input[type=text], 
        .payment-container select { 
            width: 100%; 
            padding: 10px; 
            margin-top: 5px; 
            border: 1px solid #ccc; 
            border-radius: 6px;
            box-sizing: border-box; /* Padding නිසා size එක වෙනස් වීම වැලැක්වීමට */
        }

        /* බොත්තම් වල පෙනුම */
        .payment-container .btn { 
            margin-top: 20px; 
            padding: 12px 20px; 
            background-color: #4CAF50; 
            color: white; 
            border: none; 
            cursor: pointer; 
            border-radius: 6px; 
            font-weight: bold;
            width: 100%;
            transition: 0.3s;
        }

        .payment-container .btn:hover { background-color: #45a049; }
        .payment-container .btn:disabled { background-color: #cccccc; cursor: not-allowed; }

        .payment-fields { 
            display:none; 
            margin-top: 15px; 
            padding: 15px; 
            border: 1px solid #ddd; 
            border-radius: 8px; 
            background:#fdfdfd; 
        }

        .copy-btn { 
            margin-top: 10px; 
            padding: 6px 12px; 
            background:#2196F3; 
            color:white; 
            cursor:pointer; 
            border:none; 
            border-radius: 4px; 
            font-size: 0.9rem;
        }
        
        .status-lbl { font-weight: bold; color: #007bff; display: block; margin-top: 5px; }
    </style>

    <script type="text/javascript">
        function togglePaymentFields() {
            var method = document.getElementById('<%= ddlPaymentMethod.ClientID %>').value;
            var cardFields = document.getElementById('cardFields');
            var bankFields = document.getElementById('bankFields');
            var btnRecord = document.getElementById('<%= btnRecordPayment.ClientID %>');

            cardFields.style.display = 'none';
            bankFields.style.display = 'none';
            btnRecord.disabled = true;

            if (method === 'Card') {
                cardFields.style.display = 'block';
                enableRecordButton();
            } else if (method === 'Bank Transfer') {
                bankFields.style.display = 'block';
                btnRecord.disabled = false;
            } else { // Cash
                btnRecord.disabled = false;
            }
        }

        function enableRecordButton() {
            var method = document.getElementById('<%= ddlPaymentMethod.ClientID %>').value;
            var btnRecord = document.getElementById('<%= btnRecordPayment.ClientID %>');

            if (method === 'Card') {
                var cardNumber = document.getElementById('<%= txtCardNumber.ClientID %>').value.trim();
                var cardExpiry = document.getElementById('<%= txtCardExpiry.ClientID %>').value.trim();
                var cardCVV = document.getElementById('<%= txtCardCVV.ClientID %>').value.trim();
                var cardHolder = document.getElementById('<%= txtCardHolder.ClientID %>').value.trim();

                var validNumber = cardNumber.length === 16;
                var validCVV = cardCVV.length === 3;
                var validExpiry = /^([0-1][0-9])\/([0-9]{2})$/.test(cardExpiry);

                btnRecord.disabled = !(cardHolder && validNumber && validCVV && validExpiry);
            } else {
                btnRecord.disabled = false; 
            }
        }

        function copyBankDetails() {
            var details = "Bank Name: " + document.getElementById('<%= lblBankName.ClientID %>').innerText + "\n" +
                          "Account Holder: " + document.getElementById('<%= lblBankHolder.ClientID %>').innerText + "\n" +
                          "Account Number: " + document.getElementById('<%= lblBankAccount.ClientID %>').innerText + "\n" +
                          "Branch: " + document.getElementById('<%= lblBankBranch.ClientID %>').innerText;
            navigator.clipboard.writeText(details).then(function () {
                alert("Bank details copied to clipboard!");
            });
        }

        // Page load එකේදී toggle එක නිවැරදිව පෙන්වීමට
        window.onload = function () {
            togglePaymentFields();
        }
    </script>

    <div class="payment-page-scoped">
        <div class="payment-container">
            <h2>Record Payment</h2>

            <label>Order ID:</label>
            <asp:Label ID="lblOrderID" runat="server" CssClass="status-lbl" Text=""></asp:Label>

            <label>Payment Amount:</label>
            <asp:TextBox ID="txtPaymentAmount" runat="server" placeholder="Enter amount" />

            <label>Payment Method:</label>
            <asp:DropDownList ID="ddlPaymentMethod" runat="server" onchange="togglePaymentFields();">
                <asp:ListItem Text="Cash" Value="Cash" />
                <asp:ListItem Text="Card" Value="Card" />
                <asp:ListItem Text="Bank Transfer" Value="Bank Transfer" />
            </asp:DropDownList>

            <div id="cardFields" class="payment-fields">
                <label>Card Holder Name:</label>
                <asp:TextBox ID="txtCardHolder" runat="server" onkeyup="enableRecordButton();" />

                <label>Card Number (16 digits):</label>
                <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16" onkeyup="enableRecordButton();" />

                <label>Expiry Date (MM/YY):</label>
                <asp:TextBox ID="txtCardExpiry" runat="server" MaxLength="5" onkeyup="enableRecordButton();" placeholder="MM/YY" />

                <label>CVV (3 digits):</label>
                <asp:TextBox ID="txtCardCVV" runat="server" MaxLength="3" onkeyup="enableRecordButton();" />
            </div>

            <div id="bankFields" class="payment-fields">
                <label>Bank Name:</label>
                <asp:Label ID="lblBankName" runat="server" Text="ABC Bank" Font-Bold="true"></asp:Label>

                <label>Account Holder:</label>
                <asp:Label ID="lblBankHolder" runat="server" Text="SmartRetail Pvt Ltd" Font-Bold="true"></asp:Label>

                <label>Account Number:</label>
                <asp:Label ID="lblBankAccount" runat="server" Text="1234567890" Font-Bold="true"></asp:Label>

                <label>Branch:</label>
                <asp:Label ID="lblBankBranch" runat="server" Text="Colombo Main" Font-Bold="true"></asp:Label>

                <button type="button" class="copy-btn" onclick="copyBankDetails();">📋 Copy Bank Details</button>
            </div>

            <label>Payment Status:</label>
            <asp:Label ID="lblStatus" runat="server" CssClass="status-lbl" Text="Pending"></asp:Label>

            <asp:Button ID="btnRecordPayment" runat="server" Text="💳 Record Payment" CssClass="btn" OnClick="btnRecordPayment_Click" Enabled="false" />
            
            <asp:Button ID="btnBack" runat="server" Text="🔙 Back to Dashboard" CssClass="btn" 
                OnClick="btnBack_Click" style="background-color: #6c757d; margin-top: 10px;" />
        </div>
    </div>
</asp:Content>