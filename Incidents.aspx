<%@ Page Title="Incidents" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Incidents.aspx.cs" Inherits="SACHIF_SIMS_Website.Incidents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h2><i class="fas fa-exclamation-triangle"></i> Incident Management</h2>
            <p>Report and track incidents</p>
        </div>
        <a href="ReportIncident.aspx" class="btn-danger"><i class="fas fa-plus"></i> Report Incident</a>
    </div>

    <!-- Filters -->
    <div class="card">
        <h4><i class="fas fa-filter"></i> Filter Incidents</h4>
        <div class="filter-row">
            <div class="form-group">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
                    <asp:ListItem Value="">All</asp:ListItem>
                    <asp:ListItem Value="pending">Pending</asp:ListItem>
                    <asp:ListItem Value="accepted">In Progress</asp:ListItem>
                    <asp:ListItem Value="resolved">Resolved</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Priority</label>
                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
                    <asp:ListItem Value="">All</asp:ListItem>
                    <asp:ListItem Value="Low">Low</asp:ListItem>
                    <asp:ListItem Value="High">High</asp:ListItem>
                    <asp:ListItem Value="Urgent">Urgent</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <div class="card">
        <h4><i class="fas fa-bell"></i> All Incidents</h4>
        <asp:GridView ID="gvIncidents" runat="server" AutoGenerateColumns="false" CssClass="data-grid"
            OnRowCommand="gvIncidents_RowCommand" OnRowDataBound="gvIncidents_RowDataBound"
            DataKeyNames="incident_id" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvIncidents_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="incident_id" HeaderText="ID" />
                <asp:BoundField DataField="child_name" HeaderText="Child" />
                <asp:BoundField DataField="incident_type" HeaderText="Type" />
                <asp:BoundField DataField="priority" HeaderText="Priority" />
                <asp:BoundField DataField="status" HeaderText="Status" />
                <asp:BoundField DataField="reported_at" HeaderText="Reported" DataFormatString="{0:dd MMM yyyy HH:mm}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn-view" CommandName="ViewIncident" CommandArgument='<%# Eval("incident_id") %>' />
                        <asp:Button ID="btnAccept" runat="server" Text="Accept" CssClass="btn-accept" CommandName="AcceptIncident" CommandArgument='<%# Eval("incident_id") %>' Visible='<%# Eval("status").ToString() == "pending" %>' />
                        <asp:Button ID="btnResolve" runat="server" Text="Resolve" CssClass="btn-resolve" CommandName="ResolveIncident" CommandArgument='<%# Eval("incident_id") %>' Visible='<%# Eval("status").ToString() == "accepted" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <p class="empty-msg">No incidents found.</p>
            </EmptyDataTemplate>
            <PagerStyle CssClass="pager" />
        </asp:GridView>
    </div>
</asp:Content>