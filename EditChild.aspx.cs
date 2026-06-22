using System;
using System.Web.UI;

namespace SACHIF_SIMS_Website
{
    public partial class EditChild : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}