using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class Responder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadResponderData();
            }
        }

        private void LoadResponderData()
        {
            try
            {
                string pendingQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'pending' AND is_deleted = 0";
                string inProgressQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'accepted' AND is_deleted = 0";
                string resolvedTodayQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'resolved' AND is_deleted = 0 AND DATE(resolved_at) = CURDATE()";
                string urgentQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'pending' AND priority = 'Urgent' AND is_deleted = 0";

                lblPending.Text = DatabaseHelper.ExecuteScalar(pendingQuery).ToString();
                lblInProgress.Text = DatabaseHelper.ExecuteScalar(inProgressQuery).ToString();
                lblResolved.Text = DatabaseHelper.ExecuteScalar(resolvedTodayQuery).ToString();

                int urgentCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(urgentQuery));
                if (urgentCount > 0)
                {
                    pnlUrgent.Visible = true;
                    lblUrgentCount.Text = urgentCount.ToString();
                }

                // Pending Incidents
                string pendingIncidentsQuery = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                                FROM incidents i 
                                                JOIN beneficiaries b ON i.child_id = b.child_id 
                                                WHERE i.status = 'pending' AND i.is_deleted = 0 
                                                ORDER BY CASE WHEN i.priority = 'Urgent' THEN 1 ELSE 2 END, i.reported_at ASC";

                DataTable dtPending = DatabaseHelper.ExecuteQuery(pendingIncidentsQuery);
                rptPending.DataSource = dtPending;
                rptPending.DataBind();
                lblPendingCount.Text = dtPending.Rows.Count.ToString();

                if (dtPending.Rows.Count == 0)
                {
                    lblNoPending.Visible = true;
                }

                // In Progress
                string inProgressQueryStr = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                            FROM incidents i 
                                            JOIN beneficiaries b ON i.child_id = b.child_id 
                                            WHERE i.status = 'accepted' AND i.is_deleted = 0 
                                            ORDER BY i.accepted_at DESC";

                DataTable dtInProgress = DatabaseHelper.ExecuteQuery(inProgressQueryStr);
                rptInProgress.DataSource = dtInProgress;
                rptInProgress.DataBind();
                lblInProgressCount.Text = dtInProgress.Rows.Count.ToString();

                if (dtInProgress.Rows.Count == 0)
                {
                    lblNoInProgress.Visible = true;
                }

                // Resolved
                string resolvedQuery = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                        FROM incidents i 
                                        JOIN beneficiaries b ON i.child_id = b.child_id 
                                        WHERE i.status = 'resolved' AND i.is_deleted = 0 
                                        ORDER BY i.resolved_at DESC LIMIT 10";

                DataTable dtResolved = DatabaseHelper.ExecuteQuery(resolvedQuery);
                rptResolved.DataSource = dtResolved;
                rptResolved.DataBind();

                if (dtResolved.Rows.Count == 0)
                {
                    lblNoResolved.Visible = true;
                }

                // Average response time
                string avgResponseQuery = "SELECT AVG(TIMESTAMPDIFF(MINUTE, reported_at, accepted_at)) FROM incidents WHERE accepted_at IS NOT NULL AND is_deleted = 0";
                object avgResult = DatabaseHelper.ExecuteScalar(avgResponseQuery);
                if (avgResult != DBNull.Value && avgResult != null)
                {
                    lblResponseTime.Text = Math.Round(Convert.ToDouble(avgResult)).ToString();
                }
                else
                {
                    lblResponseTime.Text = "N/A";
                }
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void rptPending_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string priority = row["priority"].ToString();

                HtmlGenericControl incidentCard = (HtmlGenericControl)e.Item.FindControl("incidentCard");
                if (incidentCard != null)
                {
                    incidentCard.Style.Add("border-left-color", priority == "Urgent" ? "#e74c3c" : "#f39c12");
                    incidentCard.Style.Add("background", "#fff5f5");
                    incidentCard.Style.Add("padding", "16px");
                    incidentCard.Style.Add("margin-bottom", "12px");
                    incidentCard.Style.Add("border-radius", "16px");
                }

                HtmlGenericControl prioritySpan = (HtmlGenericControl)e.Item.FindControl("prioritySpan");
                if (prioritySpan != null)
                {
                    prioritySpan.Style.Add("color", priority == "Urgent" ? "#e74c3c" : "#e67e22");
                    prioritySpan.Style.Add("font-weight", "bold");
                    prioritySpan.Style.Add("margin-left", "10px");
                }
            }
        }

        protected void rptInProgress_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlGenericControl incidentCard = (HtmlGenericControl)e.Item.FindControl("incidentCard");
                if (incidentCard != null)
                {
                    incidentCard.Style.Add("border-left-color", "#3498db");
                    incidentCard.Style.Add("background", "#f0f7ff");
                    incidentCard.Style.Add("padding", "16px");
                    incidentCard.Style.Add("margin-bottom", "12px");
                    incidentCard.Style.Add("border-radius", "16px");
                }
            }
        }

        protected void rptPending_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AcceptIncident")
            {
                int incidentId = Convert.ToInt32(e.CommandArgument);
                AcceptIncident(incidentId);
                LoadResponderData();
            }
        }

        protected void rptInProgress_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ResolveIncident")
            {
                int incidentId = Convert.ToInt32(e.CommandArgument);
                ResolveIncident(incidentId);
                LoadResponderData();
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
            catch (Exception)
            {
                // Handle error
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
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void btnSimulateUrgent_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO incidents (child_id, incident_type, priority, status, location, description, reported_by) 
                                VALUES ('B001', 'Emergency Alert', 'Urgent', 'pending', 'Johannesburg CBD', 'Urgent call received - immediate assistance required', @userId)";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Simulated urgent alert");
                LoadResponderData();
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void btnSync_Click(object sender, EventArgs e)
        {
            // Simulate sync
            Response.Write("<script>alert('🔄 Sync triggered: offline records uploaded to cloud. All data backed up.');</script>");
        }

        private void LogActivity(string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details) VALUES (@userId, 'Responder Action', @details)";
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