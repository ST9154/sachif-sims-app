using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SACHIF_SIMS_Website
{
    public partial class Children : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadChildren();
            }
        }

        private void LoadChildren()
        {
            try
            {
                string query = "SELECT * FROM beneficiaries WHERE is_active = 1 ORDER BY registration_date DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                gvChildren.DataSource = dt;
                gvChildren.DataBind();
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string childId = GenerateChildId();

                string query = @"INSERT INTO beneficiaries (child_id, first_name, last_name, date_of_birth, 
                                guardian_name, guardian_contact, address, registered_by) 
                                VALUES (@childId, @firstName, @lastName, @dob, @guardian, @contact, @address, @userId)";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@childId", childId),
                    new MySqlParameter("@firstName", txtFirstName.Text.Trim()),
                    new MySqlParameter("@lastName", txtLastName.Text.Trim()),
                    new MySqlParameter("@dob", string.IsNullOrEmpty(txtDOB.Text) ? DBNull.Value : (object)txtDOB.Text),
                    new MySqlParameter("@guardian", string.IsNullOrEmpty(txtGuardian.Text) ? DBNull.Value : (object)txtGuardian.Text.Trim()),
                    new MySqlParameter("@contact", string.IsNullOrEmpty(txtContact.Text) ? DBNull.Value : (object)txtContact.Text.Trim()),
                    new MySqlParameter("@address", txtAddress.Text.Trim()),
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Registered child: " + childId);

                lblMessage.Text = "✅ Child registered successfully! ID: " + childId;
                lblMessage.CssClass = "message success";
                lblMessage.Visible = true;

                // Clear form
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtDOB.Text = "";
                txtIDNum.Text = "";
                txtGuardian.Text = "";
                txtContact.Text = "";
                txtAddress.Text = "";

                LoadChildren();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error: " + ex.Message;
                lblMessage.CssClass = "message error";
                lblMessage.Visible = true;
            }
        }

        private string GenerateChildId()
        {
            string query = "SELECT COUNT(*) FROM beneficiaries WHERE child_id LIKE 'B%'";
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query)) + 101;
            return "B" + count.ToString("D3");
        }

        private void LogActivity(string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details) VALUES (@userId, 'Register Child', @details)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId),
                    new MySqlParameter("@details", details)
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch { }
        }

        protected void gvChildren_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChildren.PageIndex = e.NewPageIndex;
            LoadChildren();
        }

        protected void gvChildren_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string childId = gvChildren.DataKeys[e.NewEditIndex].Value.ToString();
            Response.Redirect("EditChild.aspx?id=" + childId);
        }

        protected void gvChildren_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string childId = gvChildren.DataKeys[e.RowIndex].Value.ToString();
            string query = "UPDATE beneficiaries SET is_active = 0 WHERE child_id = @childId";
            MySqlParameter[] parameters = {
                new MySqlParameter("@childId", childId)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            LoadChildren();
        }

        protected void gvChildren_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewChild")
            {
                string childId = e.CommandArgument.ToString();
                Response.Redirect("ViewChild.aspx?id=" + childId);
            }
        }
    }
}