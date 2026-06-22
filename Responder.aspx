<%@ Page Title="Responder" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Responder.aspx.cs" Inherits="SACHIF_SIMS_Website.Responder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Responder Hub</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Responder Operations · Emergency Tasks</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Status Bar -->
    <div class="form-card">
        <div style="display:flex; gap:12px; flex-wrap:wrap; align-items:center;">
            <span><i class="fas fa-map-marker-alt"></i> Your location: Gauteng Region</span>
            <asp:Button ID="btnSimulateUrgent" runat="server" Text="Simulate New Urgent Alert" CssClass="btn-primary" OnClick="btnSimulateUrgent_Click" />
            <asp:Button ID="btnSync" runat="server" Text="Force Sync (Offline data)" CssClass="btn-secondary" OnClick="btnSync_Click" />
        </div>
    </div>

    <!-- Stats Cards -->
    <div class="stats-grid">
        <div class="stat-card" style="border-left: 4px solid #e74c3c;">
            <i class="fas fa-bell" style="color:#e74c3c;"></i>
            <h3><asp:Label ID="lblPending" runat="server" Text="0" /></h3>
            <p>Pending Incidents</p>
        </div>
        <div class="stat-card" style="border-left: 4px solid #3498db;">
            <i class="fas fa-running" style="color:#3498db;"></i>
            <h3><asp:Label ID="lblInProgress" runat="server" Text="0" /></h3>
            <p>In Progress</p>
        </div>
        <div class="stat-card" style="border-left: 4px solid #27ae60;">
            <i class="fas fa-check-circle" style="color:#27ae60;"></i>
            <h3><asp:Label ID="lblResolved" runat="server" Text="0" /></h3>
            <p>Resolved Today</p>
        </div>
        <div class="stat-card" style="border-left: 4px solid #e67e22;">
            <i class="fas fa-clock" style="color:#e67e22;"></i>
            <h3><asp:Label ID="lblResponseTime" runat="server" Text="N/A" /></h3>
            <p>Avg Response Time (min)</p>
        </div>
    </div>

    <!-- Urgent Alert -->
    <asp:Panel ID="pnlUrgent" runat="server" Visible="false" CssClass="alert-banner urgent-alert">
        <i class="fas fa-ambulance"></i>
        <span><strong>🚨 URGENT:</strong> <asp:Label ID="lblUrgentCount" runat="server" Text="0" /> incident(s) requiring immediate attention!</span>
    </asp:Panel>

    <!-- Pending Incidents -->
    <div class="form-card">
        <h4><i class="fas fa-clock" style="color:#e74c3c;"></i> Pending Incidents (<asp:Label ID="lblPendingCount" runat="server" Text="0" />)</h4>
        <asp:Repeater ID="rptPending" runat="server" OnItemCommand="rptPending_ItemCommand" OnItemDataBound="rptPending_ItemDataBound">
            <ItemTemplate>
                <div class="incident-card" runat="server" id="incidentCard">
                    <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                        <div>
                            <strong><%# Eval("child_name") %></strong>
                            <span class="priority-label" runat="server" id="prioritySpan">🔴 <%# Eval("priority") %></span>
                            <br />
                            <span style="color:#666; font-size:13px;">
                                <i class="fas fa-tag"></i> <%# Eval("incident_type") %> &nbsp;|&nbsp;
                                <i class="fas fa-map-marker-alt"></i> <%# Eval("location") %>
                            </span>
                            <br />
                            <span style="color:#999; font-size:12px;">
                                Reported: <%# Convert.ToDateTime(Eval("reported_at")).ToString("dd MMM yyyy HH:mm") %>
                            </span>
                            <br />
                            <span style="color:#e74c3c; font-size:13px;">⚠️ <%# Eval("description").ToString().Length > 80 ? Eval("description").ToString().Substring(0, 80) + "..." : Eval("description") %></span>
                        </div>
                        <div>
                            <asp:Button ID="btnAccept" runat="server" Text="Accept & Respond" CssClass="btn-accept" 
                                CommandName="AcceptIncident" CommandArgument='<%# Eval("incident_id") %>' />
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoPending" runat="server" Text="✅ No pending incidents. Great work!" CssClass="empty-msg" Visible="false" />
    </div>

    <!-- In Progress Incidents -->
    <div class="form-card">
        <h4><i class="fas fa-running" style="color:#3498db;"></i> In Progress (<asp:Label ID="lblInProgressCount" runat="server" Text="0" />)</h4>
        <asp:Repeater ID="rptInProgress" runat="server" OnItemCommand="rptInProgress_ItemCommand" OnItemDataBound="rptInProgress_ItemDataBound">
            <ItemTemplate>
                <div class="incident-card" runat="server" id="incidentCard">
                    <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                        <div>
                            <strong><%# Eval("child_name") %></strong>
                            <span style="margin-left:10px; color:#3498db;">🔵 In Progress</span>
                            <br />
                            <span style="color:#666; font-size:13px;">
                                <i class="fas fa-tag"></i> <%# Eval("incident_type") %> &nbsp;|&nbsp;
                                <i class="fas fa-map-marker-alt"></i> <%# Eval("location") %>
                            </span>
                            <br />
                            <span style="color:#999; font-size:12px;">
                                Accepted: <%# Convert.ToDateTime(Eval("accepted_at")).ToString("dd MMM yyyy HH:mm") %>
                            </span>
                        </div>
                        <div>
                            <asp:Button ID="btnResolve" runat="server" Text="Mark Resolved" CssClass="btn-resolve" 
                                CommandName="ResolveIncident" CommandArgument='<%# Eval("incident_id") %>' />
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoInProgress" runat="server" Text="No incidents in progress." CssClass="empty-msg" Visible="false" />
    </div>

    <!-- Recently Resolved -->
    <div class="form-card">
        <h4><i class="fas fa-check-circle" style="color:#27ae60;"></i> Recently Resolved</h4>
        <asp:Repeater ID="rptResolved" runat="server">
            <ItemTemplate>
                <div class="incident-item" style="border-left: 4px solid #27ae60; background:#f0fff4; padding:10px; border-radius:12px; margin-bottom:8px;">
                    <div>
                        <strong><%# Eval("child_name") %></strong>
                        <span style="color:#666; font-size:13px; margin-left:10px;"><%# Eval("incident_type") %></span>
                        <br />
                        <span style="color:#999; font-size:12px;">
                            Resolved: <%# Convert.ToDateTime(Eval("resolved_at")).ToString("dd MMM yyyy HH:mm") %>
                        </span>
                    </div>
                    <span class="status-badge status-resolved">RESOLVED ✅</span>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoResolved" runat="server" Text="No resolved incidents yet." CssClass="empty-msg" Visible="false" />
    </div>

    <footer><i class="fas fa-phone-alt"></i> Emergency hotline: 0800 123 456 | All actions logged for audit</footer>
</asp:Content>