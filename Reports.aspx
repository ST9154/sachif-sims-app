<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="SACHIF_SIMS_Website.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Reports</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Donor Impact Proof &amp; Reporting</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hierarchical Reporting Summary -->
    <div class="stats-grid">
        <div class="stat-card">
            <i class="fas fa-chart-line"></i>
            <h3>📊 Operational</h3>
            <p>🥣 Meals Jun: 2,480<br />📚 ECD sessions: 112<br />🧑‍🏫 Counseling hrs: 89</p>
        </div>
        <div class="stat-card">
            <i class="fas fa-chart-simple"></i>
            <h3>Tactical (Monthly)</h3>
            <p>Child wellness ▲18%<br />Psychosocial score: 7.4/10<br />Case closure rate: <asp:Label ID="lblClosureRate" runat="server" Text="68%" /></p>
        </div>
        <div class="stat-card">
            <i class="fas fa-chart-pie"></i>
            <h3>Strategic ROI</h3>
            <p>SROI: 3.4x<br />Geographic reach: 14 wards<br />Fund acquittal: 96%</p>
        </div>
    </div>

    <!-- Report Filters -->
    <div class="form-card">
        <h4><i class="fas fa-filter"></i> Report Filters</h4>
        <div class="filter-row">
            <div class="form-group">
                <label>Date From</label>
                <asp:TextBox ID="txtDateFrom" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Date To</label>
                <asp:TextBox ID="txtDateTo" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Priority</label>
                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">All</asp:ListItem>
                    <asp:ListItem Value="Low">Low</asp:ListItem>
                    <asp:ListItem Value="High">High</asp:ListItem>
                    <asp:ListItem Value="Urgent">Urgent</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group" style="display:flex; align-items:flex-end; gap:8px;">
                <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter" CssClass="btn-secondary" OnClick="btnApplyFilter_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn-secondary" OnClick="btnReset_Click" />
            </div>
        </div>
    </div>

    <!-- Report Generation Buttons -->
    <div class="form-card">
        <h4><i class="fas fa-file-alt"></i> Generate Reports</h4>
        <div style="display:grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap:12px;">
            <asp:Button ID="btnDonorReport" runat="server" Text="📊 Donor Impact Report" CssClass="btn-primary" OnClick="btnDonorReport_Click" />
            <asp:Button ID="btnIncidentReport" runat="server" Text="📋 Incident Summary" CssClass="btn-secondary" OnClick="btnIncidentReport_Click" />
            <asp:Button ID="btnChildrenReport" runat="server" Text="👶 Children Report" CssClass="btn-secondary" OnClick="btnChildrenReport_Click" />
            <asp:Button ID="btnExportCSV" runat="server" Text="📥 Export All to CSV" CssClass="btn-secondary" OnClick="btnExportCSV_Click" />
        </div>
    </div>

    <!-- Report Preview -->
    <div class="form-card">
        <h4><i class="fas fa-table"></i> Report Preview</h4>
        <asp:Label ID="lblReportTitle" runat="server" Text="Select a report type above to generate" style="color:#999; display:block; margin-bottom:12px;" />
        <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="true" CssClass="data-grid" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvReport_PageIndexChanging" EmptyDataText="No data available for this report.">
            <PagerStyle CssClass="pager" />
        </asp:GridView>
    </div>

    <!-- Charts & Statistics -->
    <div style="display:grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap:16px;">
        <div class="form-card">
            <h4><i class="fas fa-chart-bar"></i> Incident by Priority</h4>
            <asp:Repeater ID="rptPriorityStats" runat="server" OnItemDataBound="rptPriorityStats_ItemDataBound">
                <ItemTemplate>
                    <div class="stat-row" runat="server" id="statRow" style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                        <span class="stat-label" runat="server" id="statLabel"><%# Eval("priority") %></span>
                        <span><%# Eval("count") %> (<%# Eval("percentage") %>%)</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="form-card">
            <h4><i class="fas fa-chart-bar"></i> Incident by Status</h4>
            <asp:Repeater ID="rptStatusStats" runat="server" OnItemDataBound="rptStatusStats_ItemDataBound">
                <ItemTemplate>
                    <div class="stat-row" runat="server" id="statRow" style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                        <span class="stat-label" runat="server" id="statLabel"><%# Eval("status").ToString().ToUpper() %></span>
                        <span><%# Eval("count") %> (<%# Eval("percentage") %>%)</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="form-card">
            <h4><i class="fas fa-chart-bar"></i> Top Incident Types</h4>
            <asp:Repeater ID="rptTypeStats" runat="server">
                <ItemTemplate>
                    <div style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                        <span><%# Eval("incident_type") %></span>
                        <span><%# Eval("count") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Compliance Statement -->
    <div class="alert-banner" style="background:#e8f8f5; border-left:5px solid #27ae60; margin-top:16px;">
        <i class="fas fa-check-circle" style="color:#27ae60;"></i>
        <span><strong>Audit-ready promise:</strong> Every transaction references Service_Catalog pricing &amp; StaffID. Normalized 3NF database ensures zero redundancy and full traceability for donor acquittal.</span>
    </div>

    <footer>📢 Reports generated from live data: incidents, service logs, inventory usage.</footer>
</asp:Content>