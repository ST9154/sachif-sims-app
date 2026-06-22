using System;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserSession.IsLoggedIn())
            {
                Response.Redirect("Dashboard.aspx");
            }

            if (!IsPostBack)
            {
                SetDemoCredentials();
            }
        }

        private void SetDemoCredentials()
        {
            string role = ddlRole.SelectedValue;
            if (role == "admin")
            {
                txtUsername.Text = "sipho@sims.com";
                txtPassword.Text = "sipho123";
            }
            else if (role == "socialworker")
            {
                txtUsername.Text = "worker@sims.com";
                txtPassword.Text = "worker123";
            }
            else if (role == "responder")
            {
                txtUsername.Text = "responder@sims.com";
                txtPassword.Text = "responder123";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = ddlRole.SelectedValue;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter username and password.";
                lblError.Visible = true;
                return;
            }

            string query = "SELECT user_id, username, full_name, role FROM users WHERE username = @username AND password = @password AND role = @role AND is_active = 1";

            MySqlParameter[] parameters = {
                new MySqlParameter("@username", username),
                new MySqlParameter("@password", password),
                new MySqlParameter("@role", role)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                UserSession.CurrentUser = new UserSession
                {
                    UserId = Convert.ToInt32(row["user_id"]),
                    Username = row["username"].ToString(),
                    FullName = row["full_name"].ToString(),
                    Role = row["role"].ToString()
                };

                LogUserActivity(UserSession.CurrentUser.UserId, "Login", "User logged in");
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                lblError.Text = "Invalid credentials. Please check your username, password, and role.";
                lblError.Visible = true;
            }
        }

        private void LogUserActivity(int userId, string action, string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details, ip_address) VALUES (@userId, @action, @details, @ip)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@action", action),
                    new MySqlParameter("@details", details),
                    new MySqlParameter("@ip", Request.UserHostAddress ?? "unknown")
                };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch { /* Logging is optional, don't break login */ }
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDemoCredentials();
        }
    }
}