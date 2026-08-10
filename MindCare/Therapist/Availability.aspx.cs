using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Therapist_Availability : TherapistBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TherapistSidebar1.ActivePage = "Availability";

        if (!IsPostBack)
        {
            LoadAvailability();
        }
    }

    private int GetTherapistId()
    {
        string query = "SELECT TherapistId FROM Therapists WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    private void LoadAvailability()
    {
        int therapistId = GetTherapistId();

        string query = @"SELECT AvailabilityId, DayOfWeek,
                                 TIME_FORMAT(StartTime, '%h:%i %p') AS StartTime,
                                 TIME_FORMAT(EndTime, '%h:%i %p') AS EndTime
                          FROM TherapistAvailability
                          WHERE TherapistId = @TherapistId AND IsActive = 1
                          ORDER BY FIELD(DayOfWeek,'Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday'), StartTime";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));

        if (dt.Rows.Count == 0)
        {
            lblNoSlots.Visible = true;
            rptAvailability.Visible = false;
        }
        else
        {
            lblNoSlots.Visible = false;
            rptAvailability.Visible = true;
            rptAvailability.DataSource = dt;
            rptAvailability.DataBind();
        }
    }

    protected void btnAddSlot_Click(object sender, EventArgs e)
    {
        TimeSpan startTime, endTime;

        if (!TimeSpan.TryParse(txtStartTime.Text, out startTime) || !TimeSpan.TryParse(txtEndTime.Text, out endTime))
        {
            lblMessage.Text = "Please choose both a start and end time.";
            return;
        }

        if (endTime <= startTime)
        {
            lblMessage.Text = "End time must be after start time.";
            return;
        }

        int therapistId = GetTherapistId();
        string day = ddlDay.SelectedValue;

        // Prevent overlapping slots on the same day (e.g. adding 10-12 when
        // 11-1 already exists would confuse the booking page's slot generator).
        string overlapQuery = @"SELECT COUNT(*) FROM TherapistAvailability
                                 WHERE TherapistId = @TherapistId AND DayOfWeek = @DayOfWeek AND IsActive = 1
                                   AND StartTime < @EndTime AND EndTime > @StartTime";

        object overlapCount = DBHelper.ExecuteScalar(overlapQuery,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@DayOfWeek", day),
            new MySqlParameter("@StartTime", startTime),
            new MySqlParameter("@EndTime", endTime));

        if (Convert.ToInt32(overlapCount) > 0)
        {
            lblMessage.Text = "This overlaps with a slot you've already added for " + day + ".";
            return;
        }

        string insertQuery = @"INSERT INTO TherapistAvailability (TherapistId, DayOfWeek, StartTime, EndTime)
                                VALUES (@TherapistId, @DayOfWeek, @StartTime, @EndTime)";

        DBHelper.ExecuteNonQuery(insertQuery,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@DayOfWeek", day),
            new MySqlParameter("@StartTime", startTime),
            new MySqlParameter("@EndTime", endTime));

        lblMessage.CssClass = "text-success";
        lblMessage.Text = "Slot added.";
        txtStartTime.Text = "";
        txtEndTime.Text = "";

        LoadAvailability();
    }

    protected void rptAvailability_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "Delete") return;

        int availabilityId = Convert.ToInt32(e.CommandArgument);
        int therapistId = GetTherapistId();

        // WHERE clause makes sure a therapist can only delete their OWN slots.
        string query = "DELETE FROM TherapistAvailability WHERE AvailabilityId = @AvailabilityId AND TherapistId = @TherapistId";

        DBHelper.ExecuteNonQuery(query,
            new MySqlParameter("@AvailabilityId", availabilityId),
            new MySqlParameter("@TherapistId", therapistId));

        LoadAvailability();
    }
}
