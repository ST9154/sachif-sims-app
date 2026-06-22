using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace SACHIF_SIMS_Website
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadCharts();
            }
        }

        private void LoadCharts()
        {
            try
            {
                // Priority statistics
                string priorityQuery = @"SELECT priority, COUNT(*) AS count, 
                                        ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM incidents WHERE is_deleted = 0), 1) AS percentage 
                                        FROM incidents WHERE is_deleted = 0 GROUP BY priority ORDER BY count DESC";
                DataTable dtPriority = DatabaseHelper.ExecuteQuery(priorityQuery);
                rptPriorityStats.DataSource = dtPriority;
                rptPriorityStats.DataBind();

                // Status statistics
                string statusQuery = @"SELECT status, COUNT(*) AS count, 
                                      ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM incidents WHERE is_deleted = 0), 1) AS percentage 
                                      FROM incidents WHERE is_deleted = 0 GROUP BY status ORDER BY count DESC";
                DataTable dtStatus = DatabaseHelper.ExecuteQuery(statusQuery);
                rptStatusStats.DataSource = dtStatus;
                rptStatusStats.DataBind();

                // Incident type statistics
                string typeQuery = @"SELECT incident_type, COUNT(*) AS count 
                                    FROM incidents WHERE is_deleted = 0 
                                    GROUP BY incident_type ORDER BY count DESC LIMIT 5";
                DataTable dtType = DatabaseHelper.ExecuteQuery(typeQuery);
                rptTypeStats.DataSource = dtType;
                rptTypeStats.DataBind();

                // Calculate closure rate
                string totalQuery = "SELECT COUNT(*) FROM incidents WHERE is_deleted = 0";
                string resolvedQuery = "SELECT COUNT(*) FROM incidents WHERE status = 'resolved' AND is_deleted = 0";
                int total = Convert.ToInt32(DatabaseHelper.ExecuteScalar(totalQuery));
                int resolved = Convert.ToInt32(DatabaseHelper.ExecuteScalar(resolvedQuery));
                if (total > 0)
                {
                    lblClosureRate.Text = Math.Round((double)resolved / total * 100) + "%";
                }
                else
                {
                    lblClosureRate.Text = "0%";
                }
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void rptPriorityStats_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string priority = row["priority"].ToString();

                HtmlGenericControl statRow = (HtmlGenericControl)e.Item.FindControl("statRow");
                HtmlGenericControl statLabel = (HtmlGenericControl)e.Item.FindControl("statLabel");

                if (statLabel != null)
                {
                    string color = priority == "Urgent" ? "#e74c3c" : (priority == "High" ? "#e67e22" : "#27ae60");
                    statLabel.Style.Add("color", color);
                    statLabel.Style.Add("font-weight", "bold");
                }
            }
        }

        protected void rptStatusStats_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string status = row["status"].ToString();

                HtmlGenericControl statRow = (HtmlGenericControl)e.Item.FindControl("statRow");
                HtmlGenericControl statLabel = (HtmlGenericControl)e.Item.FindControl("statLabel");

                if (statLabel != null)
                {
                    string color = status == "pending" ? "#f39c12" : (status == "accepted" ? "#3498db" : "#27ae60");
                    statLabel.Style.Add("color", color);
                    statLabel.Style.Add("font-weight", "bold");
                }
            }
        }

        public string GetPriorityColor(string priority)
        {
            if (priority == "Urgent") return "#e74c3c";
            if (priority == "High") return "#e67e22";
            return "#27ae60";
        }

        public string GetStatusColor(string status)
        {
            if (status == "pending") return "#f39c12";
            if (status == "accepted") return "#3498db";
            return "#27ae60";
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDateFrom.Text = "";
            txtDateTo.Text = "";
            ddlPriority.SelectedIndex = 0;
            lblReportTitle.Text = "Select a report type above to generate";
            gvReport.DataSource = null;
            gvReport.DataBind();
        }

        private void LoadReportData(string reportType = "")
        {
            try
            {
                StringBuilder query = new StringBuilder();
                string whereClause = "WHERE i.is_deleted = 0";
                string dateFrom = txtDateFrom.Text;
                string dateTo = txtDateTo.Text;
                string priority = ddlPriority.SelectedValue;

                if (!string.IsNullOrEmpty(dateFrom))
                {
                    whereClause += " AND DATE(i.reported_at) >= @dateFrom";
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    whereClause += " AND DATE(i.reported_at) <= @dateTo";
                }
                if (!string.IsNullOrEmpty(priority))
                {
                    whereClause += " AND i.priority = @priority";
                }

                var parameters = new System.Collections.Generic.List<MySqlParameter>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    parameters.Add(new MySqlParameter("@dateFrom", dateFrom));
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    parameters.Add(new MySqlParameter("@dateTo", dateTo));
                }
                if (!string.IsNullOrEmpty(priority))
                {
                    parameters.Add(new MySqlParameter("@priority", priority));
                }

                if (reportType == "donor")
                {
                    query.Append(@"SELECT 
                                    i.incident_id,
                                    CONCAT(b.first_name, ' ', b.last_name) AS child_name,
                                    i.incident_type,
                                    i.priority,
                                    i.status,
                                    i.location,
                                    DATE_FORMAT(i.reported_at, '%d %b %Y') AS reported_date,
                                    u.full_name AS reported_by
                                FROM incidents i
                                JOIN beneficiaries b ON i.child_id = b.child_id
                                LEFT JOIN users u ON i.reported_by = u.user_id
                                ");
                    query.Append(whereClause);
                    query.Append(" ORDER BY i.reported_at DESC");
                    lblReportTitle.Text = "📊 Donor Impact Report";
                }
                else if (reportType == "incident")
                {
                    query.Append(@"SELECT 
                                    i.incident_id,
                                    CONCAT(b.first_name, ' ', b.last_name) AS child_name,
                                    i.incident_type,
                                    i.priority,
                                    i.status,
                                    i.location,
                                    DATE_FORMAT(i.reported_at, '%d %b %Y %H:%i') AS reported_at,
                                    DATE_FORMAT(i.resolved_at, '%d %b %Y %H:%i') AS resolved_at,
                                    u.full_name AS reported_by
                                FROM incidents i
                                JOIN beneficiaries b ON i.child_id = b.child_id
                                LEFT JOIN users u ON i.reported_by = u.user_id
                                ");
                    query.Append(whereClause);
                    query.Append(" ORDER BY i.reported_at DESC");
                    lblReportTitle.Text = "📋 Incident Summary Report";
                }
                else if (reportType == "children")
                {
                    query.Append(@"SELECT 
                                    b.child_id,
                                    b.first_name,
                                    b.last_name,
                                    b.date_of_birth,
                                    b.guardian_name,
                                    b.guardian_contact,
                                    DATE_FORMAT(b.registration_date, '%d %b %Y') AS registered_on,
                                    (SELECT COUNT(*) FROM incidents WHERE child_id = b.child_id AND is_deleted = 0) AS incident_count
                                FROM beneficiaries b
                                WHERE b.is_active = 1
                                ORDER BY b.registration_date DESC");
                    lblReportTitle.Text = "👶 Children Report";
                }

                DataTable dt = DatabaseHelper.ExecuteQuery(query.ToString(), parameters.ToArray());
                gvReport.DataSource = dt;
                gvReport.DataBind();
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        protected void gvReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReport.PageIndex = e.NewPageIndex;
            string currentView = ViewState["CurrentReport"]?.ToString() ?? "";
            LoadReportData(currentView);
        }

        protected void btnDonorReport_Click(object sender, EventArgs e)
        {
            ViewState["CurrentReport"] = "donor";
            LoadReportData("donor");
        }

        protected void btnIncidentReport_Click(object sender, EventArgs e)
        {
            ViewState["CurrentReport"] = "incident";
            LoadReportData("incident");
        }

        protected void btnChildrenReport_Click(object sender, EventArgs e)
        {
            ViewState["CurrentReport"] = "children";
            LoadReportData("children");
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT 
                                    i.incident_id,
                                    CONCAT(b.first_name, ' ', b.last_name) AS child_name,
                                    i.incident_type,
                                    i.priority,
                                    i.status,
                                    i.location,
                                    DATE_FORMAT(i.reported_at, '%d %b %Y %H:%i') AS reported_at
                                FROM incidents i
                                JOIN beneficiaries b ON i.child_id = b.child_id
                                WHERE i.is_deleted = 0
                                ORDER BY i.reported_at DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    Response.Write("<script>alert('No data to export.');</script>");
                    return;
                }

                StringBuilder csv = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    csv.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                        csv.Append(",");
                }
                csv.AppendLine();

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string value = row[i].ToString().Replace(",", ";");
                        csv.Append(value);
                        if (i < dt.Columns.Count - 1)
                            csv.Append(",");
                    }
                    csv.AppendLine();
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment; filename=incident_report_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Error exporting data.');</script>");
            }
        }
    }
}