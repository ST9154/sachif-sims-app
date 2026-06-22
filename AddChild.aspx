<%@ Page Title="Register Child" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddChild.aspx.cs" Inherits="SACHIF_SIMS_Website.AddChild" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2><i class="fas fa-user-plus"></i> Register New Child</h2>
        <p>Enter child beneficiary information</p>
    </div>

    <div class="card form-card">
        <div class="form-row">
            <div class="form-group half">
                <label>First Name *</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter first name" />
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                    ErrorMessage="Required" CssClass="validator-error" />
            </div>
            <div class="form-group half">
                <label>Last Name *</label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter last name" />
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                    ErrorMessage="Required" CssClass="validator-error" />
            </div>
        </div>
        <div class="form-row">
            <div class="form-group half">
                <label>Date of Birth</label>
                <asp:TextBox ID="txtDOB" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="form-group half">
                <label>Guardian Name</label>
                <asp:TextBox ID="txtGuardian" runat="server" CssClass="form-control" placeholder="Guardian name" />
            </div>
        </div>
        <div class="form-row">
            <div class="form-group half">
                <label>Guardian Contact</label>
                <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" placeholder="Phone number" />
            </div>
        </div>
        <div class="form-group">
            <label>Address *</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Full address" TextMode="MultiLine" Rows="2" />
            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" 
                ErrorMessage="Address is required" CssClass="validator-error" />
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSave" runat="server" Text="Register Child" CssClass="btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCancel_Click" />
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
    </div>
</asp:Content>