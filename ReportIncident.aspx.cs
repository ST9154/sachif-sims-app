using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class ReportIncident : System.Web.UI.Page
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
                LoadIncidents();
                txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            }
        }

        private void LoadChildren()
        {
            try
            {
                string query = "SELECT child_id, first_name, last_name FROM beneficiaries WHERE is_active = 1 ORDER BY first_name";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                ddlChild.DataSource = dt;
                ddlChild.DataTextField = "first_name";
                ddlChild.DataValueField = "child_id";
                ddlChild.DataBind();

                ddlChild.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Child --", ""));
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void ddlChild_SelectedIndexChanged(object sender, EventArgs e)
        {
            string childId = ddlChild.SelectedValue;
            if (!string.IsNullOrEmpty(childId))
            {
                string query = "SELECT address FROM beneficiaries WHERE child_id = @childId";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@childId", childId)
                };
                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                txtLocation.Text = result != null ? result.ToString() : "No address on file";
            }
            else
            {
                txtLocation.Text = "";
            }
        }

        private void LoadIncidents()
        {
            try
            {
                string query = @"SELECT i.*, CONCAT(b.first_name, ' ', b.last_name) AS child_name 
                                FROM incidents i 
                                JOIN beneficiaries b ON i.child_id = b.child_id 
                                WHERE i.is_deleted = 0 
                                ORDER BY i.reported_at DESC LIMIT 10";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                rptIncidents.DataSource = dt;
                rptIncidents.DataBind();

                if (dt.Rows.Count == 0)
                {
                    lblNoIncidents.Visible = true;
                }
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void rptIncidents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string priority = row["priority"].ToString();

                HtmlGenericControl incidentRow = (HtmlGenericControl)e.Item.FindControl("incidentRow");
                if (incidentRow != null)
                {
                    string borderColor = priority == "Urgent" ? "#e74c3c" : "#f39c12";
                    string bgColor = priority == "Urgent" ? "#fef9f0" : "#fef9f0";
                    incidentRow.Style.Add("border-left", "4px solid " + borderColor);
                    incidentRow.Style.Add("background", bgColor);
                }

                HtmlGenericControl prioritySpan = (HtmlGenericControl)e.Item.FindControl("prioritySpan");
                if (prioritySpan != null)
                {
                    string color = priority == "Urgent" ? "#e74c3c" : "#e67e22";
                    prioritySpan.Style.Add("color", color);
                    prioritySpan.Style.Add("font-weight", "bold");
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string childId = ddlChild.SelectedValue;
                if (string.IsNullOrEmpty(childId))
                {
                    lblMessage.Text = "❌ Please select a child";
                    lblMessage.CssClass = "message error";
                    lblMessage.Visible = true;
                    return;
                }

                string nameQuery = "SELECT CONCAT(first_name, ' ', last_name) AS name FROM beneficiaries WHERE child_id = @childId";
                MySqlParameter[] nameParams = {
                    new MySqlParameter("@childId", childId)
                };
                string childName = DatabaseHelper.ExecuteScalar(nameQuery, nameParams).ToString();

                string query = @"INSERT INTO incidents (child_id, incident_type, priority, status, location, description, reported_by) 
                                VALUES (@childId, @type, @priority, 'pending', @location, @description, @userId)";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@childId", childId),
                    new MySqlParameter("@type", ddlIncidentType.SelectedValue),
                    new MySqlParameter("@priority", ddlPriority.SelectedValue),
                    new MySqlParameter("@location", txtLocation.Text),
                    new MySqlParameter("@description", txtDescription.Text.Trim()),
                    new MySqlParameter("@userId", UserSession.CurrentUser.UserId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LogActivity("Reported incident for: " + childName);

                string priority = ddlPriority.SelectedValue;
                string message = "🚨 Incident registered for " + childName + "!";
                if (priority == "Urgent")
                {
                    message += " Urgent alert sent to responders.";
                }

                lblMessage.Text = "✅ " + message;
                lblMessage.CssClass = "message success";
                lblMessage.Visible = true;

                txtDescription.Text = "";
                LoadIncidents();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error: " + ex.Message;
                lblMessage.CssClass = "message error";
                lblMessage.Visible = true;
            }
        }

        private void LogActivity(string details)
        {
            try
            {
                string query = "INSERT INTO service_logs (user_id, action, details) VALUES (@userId, 'Report Incident', @details)";
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