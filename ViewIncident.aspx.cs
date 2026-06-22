using System;
using System.Web.UI;

namespace SACHIF_SIMS_Website
{
    public partial class ViewIncident : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }
            Response.Write("<h2>View Incident</h2><p>This page is under construction.</p>");
        }
    }
}