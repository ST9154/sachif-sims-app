using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SACHIF_SIMS_Website
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            try
            {
                string childQuery = "SELECT COUNT(*) FROM beneficiaries WHERE is_active = 1";
                string incidentQuery = "SELECT COUNT(*) FROM incidents WHERE is_deleted = 0";
                string urgentQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'pending' AND priority = 'Urgent' AND is_deleted = 0";
                string lowStockQuery = "SELECT COUNT(*) FROM inventory WHERE quantity <= reorder_level";

                lblChildren.Text = DatabaseHelper.ExecuteScalar(childQuery).ToString();
                lblIncidents.Text = DatabaseHelper.ExecuteScalar(incidentQuery).ToString();
                lblUrgent.Text = DatabaseHelper.ExecuteScalar(urgentQuery).ToString();
                lblLowStock.Text = DatabaseHelper.ExecuteScalar(lowStockQuery).ToString();

                int urgentCount = Convert.ToInt32(lblUrgent.Text);
                if (urgentCount > 0)
                {
                    pnlUrgentAlert.Visible = true;
                    lblUrgentMsg.Text = urgentCount + " urgent incident(s) require immediate attention!";
                }

                string recentQuery = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                      FROM incidents i 
                                      JOIN beneficiaries b ON i.child_id = b.child_id 
                                      WHERE i.is_deleted = 0 
                                      ORDER BY i.reported_at DESC LIMIT 5";

                DataTable dt = DatabaseHelper.ExecuteQuery(recentQuery);
                rptRecentIncidents.DataSource = dt;
                rptRecentIncidents.DataBind();
            }
            catch (Exception)
            {
                // Handle error silently
            }
        }

        protected void rptRecentIncidents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string priority = row["priority"].ToString();

                // Find the incident div
                HtmlGenericControl incidentDiv = (HtmlGenericControl)e.Item.FindControl("incidentDiv");
                if (incidentDiv != null)
                {
                    string borderColor = priority == "Urgent" ? "#e74c3c" : "#f39c12";
                    incidentDiv.Style.Add("border-left-color", borderColor);
                    incidentDiv.Style.Add("border-left-width", "4px");
                    incidentDiv.Style.Add("border-left-style", "solid");
                    incidentDiv.Style.Add("background", priority == "Urgent" ? "#fff5f5" : "#fef9f0");
                    incidentDiv.Style.Add("padding", "16px");
                    incidentDiv.Style.Add("margin-bottom", "12px");
                    incidentDiv.Style.Add("border-radius", "16px");
                }

                // Find the priority span
                HtmlGenericControl prioritySpan = (HtmlGenericControl)e.Item.FindControl("prioritySpan");
                if (prioritySpan != null)
                {
                    string color = priority == "Urgent" ? "#e74c3c" : "#e67e22";
                    prioritySpan.Style.Add("color", color);
                    prioritySpan.Style.Add("font-weight", "bold");
                    prioritySpan.Style.Add("margin-left", "10px");
                }
            }
        }
    }
}