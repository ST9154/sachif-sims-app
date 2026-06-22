<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventory.aspx.cs" Inherits="SACHIF_SIMS_Website.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Inventory</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitle" runat="server">Resource &amp; Inventory Management</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Add New Resource -->
    <div class="form-card">
        <h3><i class="fas fa-plus-circle"></i> Add New Resource/Supply</h3>
        <div class="form-row">
            <div class="form-group">
                <label>Item Name</label>
                <asp:TextBox ID="txtItemName" runat="server" placeholder="e.g., Blankets, Sanitary pads" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Initial Quantity</label>
                <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="50" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Reorder Level</label>
                <asp:TextBox ID="txtReorder" runat="server" TextMode="Number" Text="20" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label>Supplier/Donor</label>
            <asp:TextBox ID="txtSupplier" runat="server" placeholder="Supplier name" CssClass="form-control" />
        </div>
        <asp:Button ID="btnAddItem" runat="server" Text="Add Resource" CssClass="btn-primary" OnClick="btnAddItem_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
    </div>

    <!-- Inventory List -->
    <div class="form-card">
        <h4><i class="fas fa-boxes"></i> Current Stock Levels (Audit-ready)</h4>
        <asp:GridView ID="gvInventory" runat="server" AutoGenerateColumns="false" CssClass="data-grid"
            DataKeyNames="item_id" OnRowDeleting="gvInventory_RowDeleting" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvInventory_PageIndexChanging" OnRowCommand="gvInventory_RowCommand">
            <Columns>
                <asp:BoundField DataField="item_name" HeaderText="Item" />
                <asp:BoundField DataField="quantity" HeaderText="Stock" />
                <asp:BoundField DataField="reorder_level" HeaderText="Reorder at" />
                <asp:BoundField DataField="supplier" HeaderText="Supplier" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnUse" runat="server" Text="Use 10" CssClass="btn-accept" CommandName="UseStock" CommandArgument='<%# Eval("item_id") %>' />
                        <asp:Button ID="btnRestock" runat="server" Text="Restock +50" CssClass="btn-edit" CommandName="Restock" CommandArgument='<%# Eval("item_id") %>' />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete" CommandName="Delete" OnClientClick="return confirm('Delete this item?')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <p class="empty-msg">No resources added yet.</p>
            </EmptyDataTemplate>
            <PagerStyle CssClass="pager" />
        </asp:GridView>
    </div>

    <footer>✅ Each transaction links to Service_Catalog – full donor acquittal support</footer>
</asp:Content>