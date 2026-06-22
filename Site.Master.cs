using MySql.Data.MySqlClient;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SACHIF_SIMS_Website
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
                return;
            }

            UserSession user = UserSession.CurrentUser;
            if (user != null)
            {
                // Find the Label control
                Label lblUserNameCtrl = (Label)FindControl("lblUserName");
                if (lblUserNameCtrl != null)
                {
                    lblUserNameCtrl.Text = user.FullName + " (" + user.Role + ")";
                }

                BuildNavigation(user.Role);
            }
        }

        private void BuildNavigation(string role)
        {
            // Find the PlaceHolder control
            PlaceHolder NavMenuCtrl = (PlaceHolder)FindControl("NavMenu");
            if (NavMenuCtrl == null) return;

            NavMenuCtrl.Controls.Clear();

            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath).Replace(".aspx", "");

            string navHtml = "";

            if (role == "admin")
            {
                navHtml = @"
                    <a href='Dashboard.aspx' class='nav-item" + (currentPage == "Dashboard" ? " active" : "") + @"'><i class='fas fa-tachometer-alt'></i> <span>Dashboard</span></a>
                    <a href='Children.aspx' class='nav-item" + (currentPage == "Children" ? " active" : "") + @"'><i class='fas fa-child'></i> <span>Beneficiary Intake</span></a>
                    <a href='Incidents.aspx' class='nav-item" + (currentPage == "Incidents" ? " active" : "") + @"'><i class='fas fa-exclamation-triangle'></i> <span>Incident Register</span></a>
                    <a href='Inventory.aspx' class='nav-item" + (currentPage == "Inventory" ? " active" : "") + @"'><i class='fas fa-boxes'></i> <span>Resource Mgmt</span></a>
                    <a href='Responder.aspx' class='nav-item" + (currentPage == "Responder" ? " active" : "") + @"'><i class='fas fa-bolt'></i> <span>Responder Hub</span></a>
                    <a href='Reports.aspx' class='nav-item" + (currentPage == "Reports" ? " active" : "") + @"'><i class='fas fa-chart-pie'></i> <span>Donor & Reports</span></a>
                ";
            }
            else if (role == "socialworker")
            {
                navHtml = @"
                    <a href='Dashboard.aspx' class='nav-item" + (currentPage == "Dashboard" ? " active" : "") + @"'><i class='fas fa-tachometer-alt'></i> <span>Dashboard</span></a>
                    <a href='Children.aspx' class='nav-item" + (currentPage == "Children" ? " active" : "") + @"'><i class='fas fa-child'></i> <span>Beneficiary Intake</span></a>
                    <a href='Incidents.aspx' class='nav-item" + (currentPage == "Incidents" ? " active" : "") + @"'><i class='fas fa-exclamation-triangle'></i> <span>Incident Register</span></a>
                    <a href='Inventory.aspx' class='nav-item" + (currentPage == "Inventory" ? " active" : "") + @"'><i class='fas fa-boxes'></i> <span>Resource Mgmt</span></a>
                    <a href='Reports.aspx' class='nav-item" + (currentPage == "Reports" ? " active" : "") + @"'><i class='fas fa-chart-pie'></i> <span>Donor & Reports</span></a>
                ";
            }
            else if (role == "responder")
            {
                navHtml = @"
                    <a href='Dashboard.aspx' class='nav-item" + (currentPage == "Dashboard" ? " active" : "") + @"'><i class='fas fa-tachometer-alt'></i> <span>Dashboard</span></a>
                    <a href='Incidents.aspx' class='nav-item" + (currentPage == "Incidents" ? " active" : "") + @"'><i class='fas fa-exclamation-triangle'></i> <span>Incident Register</span></a>
                    <a href='Responder.aspx' class='nav-item" + (currentPage == "Responder" ? " active" : "") + @"'><i class='fas fa-bolt'></i> <span>Responder Hub</span></a>
                    <a href='Reports.aspx' class='nav-item" + (currentPage == "Reports" ? " active" : "") + @"'><i class='fas fa-chart-pie'></i> <span>Donor & Reports</span></a>
                ";
            }

            NavMenuCtrl.Controls.Add(new LiteralControl(navHtml));
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            if (UserSession.IsLoggedIn())
            {
                try
                {
                    string query = "INSERT INTO service_logs (user_id, action, details, ip_address) VALUES (@userId, 'Logout', 'User logged out', @ip)";
                    MySqlParameter[] parameters = {
                        new MySqlParameter("@userId", UserSession.CurrentUser.UserId),
                        new MySqlParameter("@ip", Request.UserHostAddress ?? "unknown")
                    };
                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                }
                catch { /* Ignore */ }
            }
            UserSession.Logout();
            Response.Redirect("Login.aspx");
        }
    }
}