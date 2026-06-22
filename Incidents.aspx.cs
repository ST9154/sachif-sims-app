using System;
using System.Data;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class Incidents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadIncidents();
            }
        }

        private void LoadIncidents()
        {
            try
            {
                string status = ddlStatus.SelectedValue;
                string priority = ddlPriority.SelectedValue;

                string query = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                FROM incidents i 
                                JOIN beneficiaries b ON i.child_id = b.child_id 
                                WHERE i.is_deleted = 0";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND i.status = @status";
                }
                if (!string.IsNullOrEmpty(priority))
                {
                    query += " AND i.priority = @priority";
                }

                query += " ORDER BY CASE WHEN i.priority = 'Urgent' THEN 1 WHEN i.priority = 'High' THEN 2 ELSE 3 END, i.reported_at DESC";

                var parameters = new System.Collections.Generic.List<MySqlParameter>();
                if (!string.IsNullOrEmpty(status))
                {
                    parameters.Add(new MySqlParameter("@status", status));
                }
                if (!string.IsNullOrEmpty(priority))
                {
                    parameters.Add(new MySqlParameter("@priority", priority));
                }

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
                gvIncidents.DataSource = dt;
                gvIncidents.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<div class='alert error'>Error: " + ex.Message + "</div>");
            }
        }

        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            LoadIncidents();
        }

        protected void gvIncidents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvIncidents.PageIndex = e.NewPageIndex;
            LoadIncidents();
        }

        protected void gvIncidents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the priority value from the DataRow
                DataRowView row = (DataRowView)e.Row.DataItem;
                string priority = row["priority"].ToString();

                // Add CSS class based on priority
                if (priority == "Urgent")
                {
                    e.Row.Cells[3].CssClass = "priority-urgent";
                }
                else if (priority == "High")
                {
                    e.Row.Cells[3].CssClass = "priority-high";
                }
                else if (priority == "Low")
                {
                    e.Row.Cells[3].CssClass = "priority-low";
                }

                // Add CSS class based on status
                string status = row["status"].ToString();
                if (status == "pending")
                {
                    e.Row.Cells[4].CssClass = "status-pending-text";
                }
                else if (status == "accepted")
                {
                    e.Row.Cells[4].CssClass = "status-accepted-text";
                }
                else if (status == "resolved")
                {
                    e.Row.Cells[4].CssClass = "status-resolved-text";
                }
            }
        }

        protected void gvIncidents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int incidentId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewIncident")
            {
                Response.Redirect("ViewIncident.aspx?id=" + incidentId);
            }
            else if (e.CommandName == "AcceptIncident")
            {
                AcceptIncident(incidentId);
                LoadIncidents();
            }
            else if (e.CommandName == "ResolveIncident")
            {
                ResolveIncident(incidentId);
                LoadIncidents();
            }
        }

        private void AcceptIncident(int incidentId)
        {
            try
            {
                string query = @"UPDATE incidents SET status = 'accepted', accepted_by = @userId, accepted_at = NOW() 
                                WHERE incident_id = @incidentId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId),
                    new MySqlParameter("@incidentId", incidentId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Accepted incident: " + incidentId);
            }
            catch (Exception ex)
            {
                Response.Write("<div class='alert error'>Error accepting: " + ex.Message + "</div>");
            }
        }

        private void ResolveIncident(int incidentId)
        {
            try
            {
                string resolution = "Resolved - Child safe, situation handled";

                string query = @"UPDATE incidents SET status = 'resolved', resolved_by = @userId, resolved_at = NOW(), 
                                resolution_notes = @notes WHERE incident_id = @incidentId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId),
                    new MySqlParameter("@notes", resolution),
                    new MySqlParameter("@incidentId", incidentId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Resolved incident: " + incidentId);
            }
            catch (Exception ex)
            {
                Response.Write("<div class='alert error'>Error resolving: " + ex.Message + "</div>");
            }
        }

        private void LogActivity(string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details) VALUES (@userId, 'Incident Action', @details)";
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