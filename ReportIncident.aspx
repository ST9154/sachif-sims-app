<%@ Page Title="Report Incident" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportIncident.aspx.cs" Inherits="SACHIF_SIMS_Website.ReportIncident" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Report Incident</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Incident Register · Child Protection Alerts</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Log New Incident Form -->
    <div class="form-card">
        <h3><i class="fas fa-plus-circle"></i> Log New Incident</h3>
        <div class="form-row">
            <div class="form-group">
                <label>Select Child</label>
                <asp:DropDownList ID="ddlChild" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlChild_SelectedIndexChanged">
                    <asp:ListItem Value="">-- Select Child --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Incident Type</label>
                <asp:DropDownList ID="ddlIncidentType" runat="server" CssClass="form-control">
                    <asp:ListItem Value="Physical Abuse">Physical Abuse</asp:ListItem>
                    <asp:ListItem Value="Sexual Abuse">Sexual Abuse</asp:ListItem>
                    <asp:ListItem Value="Neglect">Neglect</asp:ListItem>
                    <asp:ListItem Value="Emotional Abuse">Emotional Abuse</asp:ListItem>
                    <asp:ListItem Value="Medical Neglect">Medical Neglect</asp:ListItem>
                    <asp:ListItem Value="Abandonment">Abandonment</asp:ListItem>
                    <asp:ListItem Value="Other">Other</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Priority</label>
                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-control">
                    <asp:ListItem Value="Low">Low</asp:ListItem>
                    <asp:ListItem Value="High">High</asp:ListItem>
                    <asp:ListItem Value="Urgent" Selected="True">Urgent</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Date &amp; Time</label>
                <asp:TextBox ID="txtDateTime" runat="server" TextMode="DateTimeLocal" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label>Location</label>
            <asp:TextBox ID="txtLocation" runat="server" placeholder="Gauteng region, address" CssClass="form-control" ReadOnly="true" BackColor="#f5f5f5" />
        </div>
        <div class="form-group">
            <label>Description</label>
            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Detailed narrative..." />
        </div>
        <asp:Button ID="btnSubmit" runat="server" Text="Raise Alert & Register Incident" CssClass="btn-danger" OnClick="btnSubmit_Click" />
        <div class="alert-banner urgent-alert" style="margin-top:12px;">
            <i class="fas fa-cloud-upload-alt"></i>
            <span>Incident triggers automated alarm &amp; offline storage. Responders will be notified.</span>
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
    </div>

    <!-- Recent Incidents List -->
    <div class="form-card" id="incidentListContainer">
        <h4><i class="fas fa-history"></i> Recent Incidents</h4>
        <asp:Repeater ID="rptIncidents" runat="server" OnItemDataBound="rptIncidents_ItemDataBound">
            <ItemTemplate>
                <div class="incident-row" runat="server" id="incidentRow" style="margin-bottom: 12px; padding: 10px; border-radius: 16px;">
                    <div>
                        <strong><%# Eval("child_name") %></strong> – <%# Eval("incident_type") %>
                        <span class="priority-label" runat="server" id="prioritySpan">(<%# Eval("priority") %>)</span>
                        <br />
                        <small>📅 <%# Convert.ToDateTime(Eval("reported_at")).ToString("dd MMM yyyy HH:mm") %> | 📍 <%# Eval("location") %> | Status: <%# Eval("status") %></small>
                    </div>
                    <div>
                        <asp:Label ID="lblAlert" runat="server" CssClass="btn-secondary" style="padding:2px 8px; font-size:11px;" Text="🚨 Alert active" Visible='<%# Eval("priority").ToString() == "Urgent" %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoIncidents" runat="server" Text="No incidents logged yet." CssClass="empty-msg" Visible="false" />
    </div>
</asp:Content>