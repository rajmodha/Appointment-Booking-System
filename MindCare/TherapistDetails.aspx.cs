using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class TherapistDetails : System.Web.UI.Page
{
    private int therapistId;

    protected void Page_Load(object sender, EventArgs e)
    {
        // The therapist's Id is expected in the query string, e.g. TherapistDetails.aspx?id=3
        if (!int.TryParse(Request.QueryString["id"], out therapistId))
        {
            ShowNotFound();
            return;
        }

        if (!IsPostBack)
        {
            LoadProfile();
        }
    }

    private void ShowNotFound()
    {
        pnlProfile.Visible = false;
        pnlNotFound.Visible = true;
    }

    private void LoadProfile()
    {
        string query = @"
            SELECT t.TherapistId, u.FullName, t.Qualification, t.Specialization, t.Language,
                   t.Location, t.ConsultationType, t.Fees, t.Bio, t.ProfileImage
            FROM Therapists t
            INNER JOIN Users u ON u.UserId = t.UserId
            WHERE t.TherapistId = @TherapistId AND t.ApprovalStatus = 'Approved'";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));

        if (dt.Rows.Count == 0)
        {
            ShowNotFound();
            return;
        }

        DataRow row = dt.Rows[0];
        string fullName = row["FullName"].ToString();

        litName.Text = fullName;
        litQualification.Text = row["Qualification"].ToString();
        litSpecialization.Text = row["Specialization"].ToString();
        litLanguage.Text = row["Language"].ToString();
        litLocation.Text = row["Location"].ToString();
        litConsultationType.Text = row["ConsultationType"].ToString();
        litFees.Text = row["Fees"].ToString();
        litBio.Text = string.IsNullOrWhiteSpace(row["Bio"].ToString())
            ? "This therapist hasn't added a bio yet."
            : row["Bio"].ToString();

        // Use the therapist's actual uploaded photo if they have one (see
        // Therapist/Profile.aspx.cs), otherwise fall back to the same
        // generated placeholder used everywhere else in the site.
        string profileImage = row["ProfileImage"] == DBNull.Value ? null : row["ProfileImage"].ToString();
        imgTherapist.ImageUrl = string.IsNullOrEmpty(profileImage)
            ? "https://api.dicebear.com/7.x/initials/svg?seed=" + Server.UrlEncode(fullName)
            : ResolveUrl(profileImage);

        // "Book Appointment" sends the patient straight to the booking page for this
        // therapist. If they aren't logged in, Login.aspx will redirect them back
        // once they sign in (see the ReturnUrl handling added in Login.aspx.cs).
        hlBookNow.NavigateUrl = ResolveUrl("~/Patient/BookAppointment.aspx?therapistId=" + therapistId);

        LoadAverageRating();
        LoadAvailability();
        LoadFeedback();
    }

    private void LoadAverageRating()
    {
        string query = @"SELECT AVG(Rating) AS AvgRating, COUNT(*) AS TotalReviews
                          FROM Feedback WHERE TherapistId = @TherapistId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));
        DataRow row = dt.Rows[0];

        if (row["AvgRating"] == DBNull.Value)
        {
            litRating.Text = "<span style='color:var(--mc-muted);'>No ratings yet</span>";
        }
        else
        {
            double avg = Convert.ToDouble(row["AvgRating"]);
            int total = Convert.ToInt32(row["TotalReviews"]);
            litRating.Text = "⭐ " + avg.ToString("0.0") + " (" + total + " review" + (total == 1 ? "" : "s") + ")";
        }
    }

    private void LoadAvailability()
    {
        string query = @"SELECT DayOfWeek, TIME_FORMAT(StartTime,'%h:%i %p') AS StartTime,
                                 TIME_FORMAT(EndTime,'%h:%i %p') AS EndTime
                          FROM TherapistAvailability
                          WHERE TherapistId = @TherapistId AND IsActive = 1
                          ORDER BY FIELD(DayOfWeek,'Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday')";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));

        if (dt.Rows.Count == 0)
        {
            lblNoAvailability.Visible = true;
        }
        else
        {
            rptAvailability.DataSource = dt;
            rptAvailability.DataBind();
        }
    }

    private void LoadFeedback()
    {
        string query = @"SELECT Rating, Comments FROM Feedback
                          WHERE TherapistId = @TherapistId
                          ORDER BY CreatedOn DESC LIMIT 10";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));

        if (dt.Rows.Count == 0)
        {
            lblNoFeedback.Visible = true;
        }
        else
        {
            rptFeedback.DataSource = dt;
            rptFeedback.DataBind();
        }
    }
}
