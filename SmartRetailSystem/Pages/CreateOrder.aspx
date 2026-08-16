<%@ Page Title="Create Order" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateOrder.aspx.cs" Inherits="SmartRetailSystem.CreateOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&family=Poppins:wght@800&display=swap');
        .order-wrapper { max-width: 1100px; margin: 30px auto; font-family: 'Inter', sans-serif; position: relative; }
        .page-header-container { text-align: center; margin-bottom: 35px; }
        .page-header-container h1 {
            font-family: 'Poppins', sans-serif; font-size: 3rem; font-weight: 800;
            background: linear-gradient(90deg, #1e293b, #3b82f6);
            -webkit-background-clip: text; -webkit-text-fill-color: transparent;
        }
        .floating-cart {
            position: fixed; bottom: 30px; right: 30px; width: 70px; height: 70px;
            background: linear-gradient(135deg, #3b82f6, #2563eb);
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            color: white; font-size: 1.8rem; cursor: pointer; z-index: 1000;
            box-shadow: 0 10px 25px rgba(37, 99, 235, 0.4);
            transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
        }
        .cart-badge {
            position: absolute; top: 0; right: 0; background: #ef4444; color: white; 
            font-size: 0.8rem; font-weight: 700; min-width: 24px; height: 24px; 
            border-radius: 12px; display: flex; align-items: center; justify-content: center;
            border: 2px solid white; padding: 0 5px;
        }
        .cart-drawer {
            position: fixed; top: 0; right: -450px; width: 420px; height: 100%;
            background: white; z-index: 1001; box-shadow: -10px 0 30px rgba(0,0,0,0.1);
            transition: right 0.4s cubic-bezier(0.4, 0, 0.2, 1); padding: 25px; display: flex; flex-direction: column;
        }
        .cart-drawer.open { right: 0; }
        .drawer-overlay {
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(15, 23, 42, 0.5); z-index: 1000; display: none; backdrop-filter: blur(4px);
        }
        .status-card { background: white; padding: 20px; border-radius: 12px; border-left: 5px solid #3b82f6; box-shadow: 0 4px 6px rgba(0,0,0,0.05); margin-bottom: 25px; border: 1px solid #f1f5f9; }
        .product-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 20px; }
        .product-card { background: white; border-radius: 15px; padding: 18px; border: 1px solid #e2e8f0; transition: 0.3s; position: relative; }
        
        /* Slide Toggle Styles */
        .expand-panel { max-height: 0; overflow: hidden; transition: all 0.4s ease-out; opacity: 0; }
        .expand-panel.active { max-height: 500px; margin-top: 15px; opacity: 1; }
        
        .custom-input { width: 100%; padding: 10px; border: 1px solid #e2e8f0; border-radius: 8px; margin-bottom: 5px; }
        .btn-add { background: #3b82f6; color: white; border: none; padding: 10px; border-radius: 8px; font-weight: 600; cursor: pointer; transition: 0.2s; width: 100%; }
        .btn-confirm { background: #10b981; color: white; border: none; padding: 10px; border-radius: 8px; font-weight: 600; cursor: pointer; width: 100%; margin-top: 5px; }
        .modern-table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
        .modern-table th { background: #f8fafc; padding: 12px; text-align: left; border-bottom: 2px solid #e2e8f0; color: #64748b; }
        .modern-table td { padding: 12px; border-bottom: 1px solid #f1f5f9; }
        .drawer-footer { margin-top: auto; background: #1e293b; color: white; padding: 20px; border-radius: 12px; }
        .quick-summary { padding: 12px; background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 10px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="order-wrapper">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <asp:UpdatePanel ID="upBadge" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="floating-cart" id="cartToggleBtn" onclick="toggleCartDrawer()">
                    <i class="fas fa-shopping-cart"></i>
                    <div class="cart-badge"><asp:Literal ID="litCartCount" runat="server" Text="0"></asp:Literal></div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div class="drawer-overlay" id="cartOverlay" onclick="toggleCartDrawer()"></div>

        <div class="cart-drawer" id="cartDrawer">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; border-bottom: 1px solid #f1f5f9; padding-bottom: 15px;">
                <h3 style="margin:0; font-weight: 800; color: #1e293b;">🛒 Your Cart</h3>
                <button type="button" style="border:none; background:none; font-size: 1.5rem; cursor:pointer;" onclick="toggleCartDrawer()">&times;</button>
            </div>
            <div style="flex-grow: 1; overflow-y: auto;">
                <asp:UpdatePanel ID="UpdatePanelCart" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="false" CssClass="modern-table" DataKeyNames="ProductID" OnRowCommand="gvCart_RowCommand" GridLines="None">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductName" HeaderText="Item" />
                                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="Rs. {0:N2}" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Container.DataItemIndex %>' ForeColor="#ef4444">
                                            <i class="fas fa-trash-alt"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div style="text-align:center; padding:40px; color: #94a3b8;">Cart is empty</div>
                            </EmptyDataTemplate>
                        </asp:GridView>

                        <div class="drawer-footer">
                            <div style="display: flex; justify-content: space-between; margin-bottom: 5px;">
                                <span style="color: #94a3b8; font-size: 0.9rem;">Subtotal:</span>
                                <span style="font-weight: 600; color: white;">Rs. <asp:Label ID="lblCartSubtotal" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                            <div style="display: flex; justify-content: space-between; margin-bottom: 10px; border-bottom: 1px solid #334155; padding-bottom: 10px;">
                                <span style="color: #94a3b8; font-size: 0.9rem;">Tax (10%):</span>
                                <span style="font-weight: 600; color: white;">Rs. <asp:Label ID="lblCartTax" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                            <div style="display: flex; justify-content: space-between; margin-bottom: 15px;">
                                <span style="color: #94a3b8;">Grand Total:</span>
                                <span style="font-weight: 700; color: #38bdf8; font-size: 1.2rem;">Rs. <asp:Label ID="lblGrandTotal" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                            <asp:Button ID="btnConfirmOrder" runat="server" Text="CONFIRM ORDER ➔" OnClick="btnConfirmOrder_Click" CssClass="btn-add" style="background: linear-gradient(135deg, #10b981, #059669); height: 50px;" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <div class="page-header-container">
            <h1>🛒 New Order Entry</h1>
        </div>

        <asp:UpdatePanel ID="UpdatePanelOrder" runat="server">
            <ContentTemplate>
                <div class="status-card">
                    <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap;">
                        <div style="flex: 1;">
                            <span style="color: #64748b; font-size: 0.8rem; font-weight: 600;">Customer Information</span><br />
                            <asp:PlaceHolder ID="phAdminView" runat="server" Visible="false">
                                <asp:DropDownList ID="ddlCustomers" runat="server" CssClass="custom-input" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" style="width: 300px; margin-top:5px;"></asp:DropDownList>
                            </asp:PlaceHolder>
                            <asp:Label ID="lblCustomerName" runat="server" style="font-weight:700; color:#1e293b; font-size:1.1rem;"></asp:Label>
                        </div>
                        <div style="text-align: right;">
                            <span style="color: #64748b; font-size: 0.8rem; font-weight: 600;">Available Credit</span><br />
                            <asp:Label ID="lblAvailableCredit" runat="server" style="font-weight:700; font-size:1.1rem;" Text="Rs. 0.00"></asp:Label>
                        </div>
                    </div>
                    <asp:Panel ID="pnlBlockMessage" runat="server" CssClass="block-alert" Visible="false" style="background-color: #fef2f2; border: 1px solid #fee2e2; color: #b91c1c; padding: 15px; border-radius: 8px; margin-top: 15px; font-weight: 600; display: flex; align-items: center; gap: 10px;">
                        <i class="fas fa-exclamation-triangle"></i> 
                        <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                    </asp:Panel>
                </div>

                <h3 style="font-weight: 700; margin-bottom: 20px;">📦 Select Products</h3>
                <div class="product-grid">
                    <asp:Repeater ID="rptProducts" runat="server">
                        <ItemTemplate>
                            <div class="product-card">
                                <div style="height: 120px; display: flex; align-items: center; justify-content: center; background: #f8fafc; border-radius: 10px; margin-bottom: 10px;">
                                    <img src='<%# ResolveUrl(Eval("ImagePath") != null ? Eval("ImagePath").ToString() : "~/Images/no-image.png") %>' style="max-height: 100px; object-fit: contain;" />
                                </div>
                                <h5 style="margin: 0; font-weight: 700;"><%# Eval("ProductName") %></h5>
                                <div style="display: flex; justify-content: space-between; margin: 10px 0;">
                                    <span style="color: #2563eb; font-weight: 800;">Rs. <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price", "{0:N2}") %>'></asp:Label></span>
                                    <span style="font-size: 0.75rem; color: #64748b;">Stock: <%# Eval("StockQuantity") %></span>
                                </div>
                                
                                <asp:TextBox ID="txtQty" runat="server" TextMode="Number" Text="1" CssClass="custom-input" style="text-align:center;"></asp:TextBox>
                                
                                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 5px;">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add to Cart" CssClass="btn-add" CommandArgument='<%# Eval("ProductID") %>' OnClick="btnAddProduct_Click" OnClientClick="animateCart()" />
                                    <asp:Button ID="btnDirect" runat="server" Text="Confirm Now ➔" CssClass="btn-confirm" CommandArgument='<%# Eval("ProductID") %>' OnClick="btnDirectOrder_Click" OnClientClick="return toggleConfirmPanel(this);" />
                                </div>

                                <asp:Panel ID="pnlDirectSummary" runat="server" CssClass="expand-panel">
                                    <div class="quick-summary">
                                        <div style="font-size: 0.75rem;">
                                            <div style="display: flex; justify-content: space-between; color: #065f46;">
                                                <span>Subtotal:</span>
                                                <span style="font-weight: 600;">Rs. <asp:Label ID="lblDirectSub" runat="server"></asp:Label></span>
                                            </div>
                                            <div style="display: flex; justify-content: space-between; color: #065f46;">
                                                <span>Tax (10%):</span>
                                                <span style="font-weight: 600;">Rs. <asp:Label ID="lblDirectTax" runat="server"></asp:Label></span>
                                            </div>
                                            <hr style="border:0; border-top: 1px solid #a7f3d0; margin: 5px 0;" />
                                            <div style="display: flex; justify-content: space-between; font-weight: 800; color: #047857; font-size: 0.85rem; margin-bottom: 8px;">
                                                <span>Grand Total:</span>
                                                <span>Rs. <asp:Label ID="lblDirectTotal" runat="server"></asp:Label></span>
                                            </div>
                                        </div>
                                        <asp:Button ID="btnGoToPayment" runat="server" Text="PROCEED TO PAYMENT ➔" 
                                            CssClass="btn-confirm" OnClick="btnGoToPayment_Click" 
                                            style="background: #059669; font-size: 0.7rem; height: 35px; border-radius: 6px;" />
                                    </div>
                                </asp:Panel>

                                <button type="button" style="width:100%; border:none; background:#f1f5f9; margin-top:8px; padding:8px; border-radius:5px; font-size:0.75rem; cursor:pointer;" onclick='toggleExpand(this)'>View Details <i class="fas fa-chevron-down"></i></button>
                                <div class="expand-panel">
                                    <p style="font-size: 0.8rem; color:#475569; margin:0;"><%# Eval("Description") %></p>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <script type="text/javascript">
        function toggleCartDrawer() {
            var drawer = document.getElementById('cartDrawer');
            var overlay = document.getElementById('cartOverlay');
            drawer.classList.toggle('open');
            overlay.style.display = drawer.classList.contains('open') ? 'block' : 'none';
        }

        function animateCart() {
            var cartIcon = document.getElementById('cartToggleBtn');
            cartIcon.style.transform = "scale(1.2)";
            setTimeout(function () { cartIcon.style.transform = "scale(1)"; }, 300);
        }

        // View Details Toggle
        function toggleExpand(btn) {
            var panel = btn.nextElementSibling;
            panel.classList.toggle('active');
            if (panel.classList.contains('active')) {
                btn.innerHTML = 'Hide Details <i class="fas fa-chevron-up"></i>';
            } else {
                btn.innerHTML = 'View Details <i class="fas fa-chevron-down"></i>';
            }
        }

        // Confirm Order Toggle Logic
        function toggleConfirmPanel(btn) {
            var card = btn.closest('.product-card');
            var panel = card.querySelector('.expand-panel'); // Summary panel follows CSS logic

            // If panel is already visible, hide it and prevent server postback
            if (panel.classList.contains('active')) {
                panel.classList.remove('active');
                btn.innerHTML = 'Confirm Now ➔';
                return false;
            }
            // Otherwise, allow postback to calculate totals
            return true;
        }
    </script>
</asp:Content>