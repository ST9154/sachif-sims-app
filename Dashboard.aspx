<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SACHIF_SIMS_Website.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Dashboard</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Dashboard · Real-time Overview</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Stats Grid -->
    <div class="stats-grid">
        <div class="stat-card">
            <i class="fas fa-child"></i>
            <h3><asp:Label ID="lblChildren" runat="server" Text="0" /></h3>
            <p>Registered Children</p>
        </div>
        <div class="stat-card">
            <i class="fas fa-folder-open"></i>
            <h3><asp:Label ID="lblIncidents" runat="server" Text="0" /></h3>
            <p>Open Incidents</p>
        </div>
        <div class="stat-card">
            <i class="fas fa-bell"></i>
            <h3><asp:Label ID="lblUrgent" runat="server" Text="0" /></h3>
            <p>Urgent Alerts</p>
        </div>
        <div class="stat-card">
            <i class="fas fa-boxes"></i>
            <h3><asp:Label ID="lblLowStock" runat="server" Text="0" /></h3>
            <p>Low Stock Items</p>
        </div>
    </div>

    <!-- Urgent Alert -->
    <asp:Panel ID="pnlUrgentAlert" runat="server" Visible="false" CssClass="alert-banner urgent-alert">
        <i class="fas fa-ambulance"></i>
        <span><strong>CRITICAL:</strong> <asp:Label ID="lblUrgentMsg" runat="server" Text="Urgent incident requires immediate attention!" /></span>
    </asp:Panel>

    <!-- Recent Incidents -->
    <div class="card">
        <h4><i class="fas fa-bell" style="color:#e67e22;"></i> Safety-First Alerts</h4>
        <asp:Repeater ID="rptRecentIncidents" runat="server" OnItemDataBound="rptRecentIncidents_ItemDataBound">
            <ItemTemplate>
                <div class="incident-card" runat="server" id="incidentDiv">
                    <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                        <div>
                            <strong><%# Eval("child_name") %></strong>
                            <span class="priority-label" runat="server" id="prioritySpan">
                                <%# Eval("priority") %>
                            </span>
                            <br />
                            <small><%# Eval("incident_type") %> | 📍 <%# Eval("location") %></small>
                            <br />
                            <small><%# Convert.ToDateTime(Eval("reported_at")).ToString("dd MMM yyyy HH:mm") %></small>
                        </div>
                        <span class="status-badge status-<%# Eval("status") %>"><%# Eval("status").ToString().ToUpper() %></span>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div style="padding:12px; background:#fff4e6; border-radius:20px; margin-top:8px;">
            📊 Operational: <asp:Label ID="lblOperational" runat="server" Text="Meals served today: 184 | ECD sessions: 12" />
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="card">
        <h4><i class="fas fa-bolt"></i> Quick Actions</h4>
        <div class="quick-actions">
            <a href="AddChild.aspx" class="btn-primary"><i class="fas fa-user-plus"></i> Register Child</a>
            <a href="ReportIncident.aspx" class="btn-danger"><i class="fas fa-exclamation-triangle"></i> Report Incident</a>
            <a href="Inventory.aspx" class="btn-secondary"><i class="fas fa-boxes"></i> Manage Stock</a>
        </div>
    </div>
</asp:Content>