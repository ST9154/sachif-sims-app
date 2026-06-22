using System;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class AddChild : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
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
                txtGuardian.Text = "";
                txtContact.Text = "";
                txtAddress.Text = "";
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Children.aspx");
        }
    }
}