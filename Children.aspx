<%@ Page Title="Children" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Children.aspx.cs" Inherits="SACHIF_SIMS_Website.Children" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Children</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Beneficiary Intake &amp; 3NF Registry</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Register New Child Form -->
    <div class="form-card" id="beneficiaryFormContainer">
        <h3><i class="fas fa-user-plus"></i> Register New Child</h3>
        <div class="form-row">
            <div class="form-group">
                <label>First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" placeholder="First name" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                    ErrorMessage="Required" CssClass="validator-error" />
            </div>
            <div class="form-group">
                <label>Last Name</label>
                <asp:TextBox ID="txtLastName" runat="server" placeholder="Last name" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                    ErrorMessage="Required" CssClass="validator-error" />
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Date of Birth</label>
                <asp:TextBox ID="txtDOB" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>ID Number</label>
                <asp:TextBox ID="txtIDNum" runat="server" placeholder="ID number" CssClass="form-control" />
            </div>
        </div>
        <div class="form-row">
            <div class="form-group">
                <label>Guardian Name</label>
                <asp:TextBox ID="txtGuardian" runat="server" placeholder="Guardian name" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Guardian Contact</label>
                <asp:TextBox ID="txtContact" runat="server" placeholder="Phone number" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label>Address</label>
            <asp:TextBox ID="txtAddress" runat="server" placeholder="Full address" TextMode="MultiLine" Rows="2" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" 
                ErrorMessage="Address is required" CssClass="validator-error" />
        </div>
        <asp:Button ID="btnSave" runat="server" Text="Save (Offline-ready)" CssClass="btn-primary" OnClick="btnSave_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
    </div>

    <!-- Registered Children List -->
    <div class="form-card" id="beneficiaryList">
        <h4><i class="fas fa-list"></i> Registered Children</h4>
        <asp:GridView ID="gvChildren" runat="server" AutoGenerateColumns="false" CssClass="data-grid"
            OnRowEditing="gvChildren_RowEditing" OnRowDeleting="gvChildren_RowDeleting"
            OnRowCommand="gvChildren_RowCommand" DataKeyNames="child_id" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvChildren_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="child_id" HeaderText="ID" />
                <asp:BoundField DataField="first_name" HeaderText="First Name" />
                <asp:BoundField DataField="last_name" HeaderText="Last Name" />
                <asp:BoundField DataField="guardian_name" HeaderText="Guardian" />
                <asp:BoundField DataField="guardian_contact" HeaderText="Contact" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn-view" CommandName="ViewChild" CommandArgument='<%# Eval("child_id") %>' />
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-edit" CommandName="Edit" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete" CommandName="Delete" OnClientClick="return confirm('Delete this child?')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <p class="empty-msg">No children registered yet.</p>
            </EmptyDataTemplate>
            <PagerStyle CssClass="pager" />
        </asp:GridView>
    </div>
</asp:Content>