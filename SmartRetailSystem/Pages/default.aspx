<%@ Page Title="Welcome | Smart Retail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SmartRetailSystem.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Modern Hero Section */
        .hero-banner {
            background: linear-gradient(135deg, #0f172a 0%, #2563eb 100%);
            border-radius: 24px;
            padding: 6rem 2rem;
            color: white;
            text-align: center;
            margin-bottom: 4rem;
            box-shadow: 0 20px 40px rgba(15, 23, 42, 0.2);
            position: relative;
            overflow: hidden;
        }

        .hero-banner::after {
            content: "";
            position: absolute;
            top: -50%;
            right: -10%;
            width: 400px;
            height: 400px;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 50%;
        }

        .hero-banner h1 { 
            font-size: 3.5rem; 
            font-weight: 800; 
            margin-bottom: 1.5rem; 
            letter-spacing: -2px;
            line-height: 1.1;
        }
        
        .hero-banner p { 
            font-size: 1.25rem; 
            color: #e2e8f0; 
            max-width: 700px; 
            margin: 0 auto 2.5rem; 
            line-height: 1.6;
        }

        /* Buttons */
        .btn-custom {
            padding: 14px 40px;
            border-radius: 12px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.3s ease;
            display: inline-block;
            font-size: 1rem;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .btn-get-started {
            background-color: #ffffff;
            color: #0f172a;
        }

        .btn-get-started:hover {
            transform: translateY(-3px);
            background-color: #f1f5f9;
            box-shadow: 0 10px 20px rgba(0,0,0,0.1);
        }

        .btn-outline-white {
            background: transparent;
            border: 2px solid rgba(255,255,255,0.3);
            color: white;
            margin-left: 15px;
        }

        /* Section Styling */
        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 2.5rem;
            border-left: 6px solid #2563eb;
            padding-left: 20px;
        }

        .section-title { 
            font-size: 2rem;
            font-weight: 800; 
            color: #0f172a; 
            margin: 0;
        }

        /* Responsive Product Grid */
        .product-grid { 
            display: grid; 
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); 
            gap: 2rem; 
            margin-bottom: 5rem;
        }

        /* Professional Product Card */
        .product-card {
            background: white;
            border-radius: 20px;
            border: 1px solid #f1f5f9;
            overflow: hidden;
            transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
            display: flex;
            flex-direction: column;
            height: 100%;
        }

        .product-card:hover { 
            transform: translateY(-12px); 
            box-shadow: 0 30px 60px -12px rgba(15, 23, 42, 0.15); 
        }

        .image-container {
            width: 100%;
            height: 250px;
            overflow: hidden;
            background-color: #f8fafc;
            display: flex;
            align-items: center;
            justify-content: center;
            border-bottom: 1px solid #f1f5f9;
        }

        .product-image { 
            width: 100%; 
            height: 100%; 
            object-fit: contain; /* object-fit කවර් වෙනුවට contain දැමීමෙන් රූපය කැපී නොයයි */
            transition: 0.5s;
            padding: 10px;
        }

        .product-card:hover .product-image {
            transform: scale(1.05);
        }

        .product-details { 
            padding: 1.5rem; 
            display: flex;
            flex-direction: column;
            flex-grow: 1;
        }
        
        .product-name { 
            font-weight: 700; 
            font-size: 1.2rem; 
            color: #1e293b; 
            margin-bottom: 0.5rem;
            height: 3rem; /* පෙළ පේළි දෙකකට සීමා කර Grid එක එක සමානව තබා ගැනීමට */
            overflow: hidden;
        }
        
        .product-price { 
            color: #2563eb; 
            font-weight: 800; 
            font-size: 1.4rem; 
            margin-bottom: 1.5rem; 
        }

        .btn-buy-now {
            width: 100%;
            background-color: #0f172a;
            color: white;
            border: none;
            padding: 12px;
            border-radius: 12px;
            font-weight: 700;
            cursor: pointer;
            transition: 0.3s;
            text-transform: uppercase;
            margin-top: auto;
        }

        .btn-buy-now:hover { 
            background-color: #2563eb; 
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="hero-banner">
        <h1>Future of Smart Retailing</h1>
        <p>Experience the most advanced integrated retail management system. Quality products, seamless payments, and lightning-fast delivery.</p>
        <div class="d-flex justify-content-center">
            <a href="Signup.aspx" class="btn-custom btn-get-started">Create Account</a>
            <a href="Login.aspx" class="btn-custom btn-outline-white">Member Sign In</a>
        </div>
    </div>

    <div class="container-fluid px-4">
        <div class="section-header">
            <h2 class="section-title">Trending Now</h2>
            <span class="badge bg-primary rounded-pill px-3 py-2">Hot Picks</span>
        </div>
        
        <div class="product-grid">
            <asp:Repeater ID="rptTrending" runat="server">
                <ItemTemplate>
                    <div class="product-card">
                        <div class="image-container">
    <img src='<%# GetProductImage(Eval("ImagePath")) %>' 
         class="product-image" 
         alt='<%# Eval("ProductName") %>' 
         onerror="this.src='https://via.placeholder.com/300x250?text=Path+Error';" />
</div>
                        
                        <div class="product-details text-center">
                            <h5 class="product-name"><%# Eval("ProductName") %></h5>
                            <span class="product-price">LKR <%# String.Format("{0:N2}", Eval("Price")) %></span>
                            
                            <asp:Button ID="btnBuy" runat="server" 
                                Text="Purchase Now" 
                                CssClass="btn-buy-now" 
                                CommandArgument='<%# Eval("ProductID") %>' 
                                OnClick="btnBuy_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>