using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace SACHIF_SIMS_Website
{
    public partial class Inventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadInventory();
            }
        }

        private void LoadInventory()
        {
            try
            {
                string query = "SELECT * FROM inventory ORDER BY item_name";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                gvInventory.DataSource = dt;
                gvInventory.DataBind();
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtItemName.Text))
                {
                    lblMessage.Text = "❌ Please enter item name";
                    lblMessage.CssClass = "message error";
                    lblMessage.Visible = true;
                    return;
                }

                int quantity = string.IsNullOrEmpty(txtQuantity.Text) ? 0 : Convert.ToInt32(txtQuantity.Text);
                int reorder = string.IsNullOrEmpty(txtReorder.Text) ? 20 : Convert.ToInt32(txtReorder.Text);

                string query = "INSERT INTO inventory (item_name, quantity, reorder_level, supplier) VALUES (@name, @qty, @reorder, @supplier)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@name", txtItemName.Text.Trim()),
                    new MySqlParameter("@qty", quantity),
                    new MySqlParameter("@reorder", reorder),
                    new MySqlParameter("@supplier", string.IsNullOrEmpty(txtSupplier.Text) ? "Local Donor" : txtSupplier.Text.Trim())
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);

                lblMessage.Text = "✅ Resource added successfully!";
                lblMessage.CssClass = "message success";
                lblMessage.Visible = true;

                txtItemName.Text = "";
                txtQuantity.Text = "50";
                txtReorder.Text = "20";
                txtSupplier.Text = "";
                LoadInventory();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error: " + ex.Message;
                lblMessage.CssClass = "message error";
                lblMessage.Visible = true;
            }
        }

        protected void gvInventory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInventory.PageIndex = e.NewPageIndex;
            LoadInventory();
        }

        protected void gvInventory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int itemId = Convert.ToInt32(gvInventory.DataKeys[e.RowIndex].Value);
            string query = "DELETE FROM inventory WHERE item_id = @id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@id", itemId)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            LoadInventory();
        }

        protected void gvInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "UseStock")
            {
                UseStock(itemId);
                LoadInventory();
            }
            else if (e.CommandName == "Restock")
            {
                Restock(itemId);
                LoadInventory();
            }
        }

        private void UseStock(int itemId)
        {
            try
            {
                string query = "UPDATE inventory SET quantity = quantity - 10 WHERE item_id = @id AND quantity >= 10";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@id", itemId)
                };
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected == 0)
                {
                    Response.Write("<div class='alert error'>Insufficient stock!</div>");
                }
                else
                {
                    LogActivity("Used 10 items from stock");
                }
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        private void Restock(int itemId)
        {
            try
            {
                string query = "UPDATE inventory SET quantity = quantity + 50 WHERE item_id = @id";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@id", itemId)
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Restocked 50 items");
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        private void LogActivity(string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details) VALUES (@userId, 'Inventory Action', @details)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId),
                    new MySqlParameter("@details", details)
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch { }
        }
    }
}