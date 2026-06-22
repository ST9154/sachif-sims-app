<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SACHIF_SIMS_Website.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SIMS Login | SACHIF</title>
    <link rel="stylesheet" href="CSS/Style.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <style>
        body {
            overflow: auto;
            background: linear-gradient(135deg, #0b2b3b 0%, #1b4f6e 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100vh;
            margin: 0;
            font-family: 'Segoe UI', Roboto, 'Helvetica Neue', sans-serif;
        }
        .login-card {
            background: white;
            border-radius: 36px;
            padding: 2.5rem 2rem;
            width: 420px;
            max-width: 90%;
            text-align: center;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
        }
        .login-card i.login-icon {
            font-size: 56px;
            color: #e67e22;
            margin-bottom: 12px;
        }
        .login-card h1 {
            color: #0f2c38;
            font-size: 28px;
            margin-bottom: 4px;
        }
        .login-card .subtitle {
            color: #7f8c8d;
            font-size: 14px;
            margin-bottom: 28px;
        }
        .input-group {
            text-align: left;
            margin-bottom: 16px;
        }
        .input-group label {
            display: block;
            font-weight: 600;
            font-size: 13px;
            color: #2c3e44;
            margin-bottom: 6px;
        }
        .input-group input,
        .input-group select {
            width: 100%;
            padding: 12px 16px;
            border: 1px solid #cbd5e1;
            border-radius: 28px;
            font-size: 14px;
            background: #fff;
            transition: border-color 0.2s;
        }
        .input-group input:focus,
        .input-group select:focus {
            outline: none;
            border-color: #e67e22;
            box-shadow: 0 0 0 3px rgba(230, 126, 34, 0.1);
        }
        .login-btn {
            width: 100%;
            background: #e67e22;
            border: none;
            padding: 14px;
            border-radius: 40px;
            color: white;
            font-weight: bold;
            font-size: 16px;
            cursor: pointer;
            transition: background 0.3s;
            margin-top: 8px;
        }
        .login-btn:hover {
            background: #d35400;
        }
        .error-msg {
            color: #e74c3c;
            font-size: 13px;
            margin-top: 12px;
            display: block;
        }
        .demo-creds {
            margin-top: 20px;
            padding: 14px;
            background: #f0f7f0;
            border-radius: 16px;
            font-size: 12px;
            text-align: left;
        }
        .demo-creds p {
            margin: 4px 0;
            font-family: monospace;
        }
        .login-footer {
            margin-top: 20px;
            font-size: 12px;
            color: #95a5a6;
        }
        .login-footer i {
            color: #27ae60;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <i class="fas fa-hands-helping login-icon"></i>
            <h1>SACHIF SIMS</h1>
            <p class="subtitle">Social Incident Management System</p>

            <div class="input-group">
                <label><i class="fas fa-user"></i> Role</label>
                <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                    <asp:ListItem Value="admin">👑 Admin / Manager</asp:ListItem>
                    <asp:ListItem Value="socialworker">👤 Social Worker</asp:ListItem>
                    <asp:ListItem Value="responder">🚑 Responder</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="input-group">
                <label><i class="fas fa-envelope"></i> Username</label>
                <asp:TextBox ID="txtUsername" runat="server" placeholder="Enter your username" />
            </div>

            <div class="input-group">
                <label><i class="fas fa-lock"></i> Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login →" CssClass="login-btn" OnClick="btnLogin_Click" />

            <asp:Label ID="lblError" runat="server" CssClass="error-msg" Visible="false" />

            <div class="demo-creds">
                <strong>🔑 Demo Credentials:</strong>
                <p>👑 Admin: sipho@sims.com / sipho123</p>
                <p>👤 Social Worker: worker@sims.com / worker123</p>
                <p>🚑 Responder: responder@sims.com / responder123</p>
            </div>

            <div class="login-footer">
                <i class="fas fa-shield-alt"></i> POPIA Compliant | <i class="fas fa-cloud-upload-alt"></i> Offline-first
            </div>
        </div>
    </form>
</body>
</html>