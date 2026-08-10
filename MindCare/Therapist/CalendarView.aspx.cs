using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Therapist_CalendarView : TherapistBasePage
{
    private Dictionary<DateTime, int> appointmentDayCounts = new Dictionary<DateTime, int>();

    protected void Page_Load(object sender, EventArgs e)
    {
        TherapistSidebar1.ActivePage = "Calendar";

        // See Patient/CalendarView.aspx.cs for the full explanation - VisibleDate
        // defaults to DateTime.MinValue until the calendar is actually navigated,
        // even though it visually renders the current month regardless.
        DateTime effectiveMonth = calAppointments.VisibleDate == DateTime.MinValue
            ? calAppointments.TodaysDate
            : calAppointments.VisibleDate;

        LoadMonthAppointmentCounts(effectiveMonth);

        if (!IsPostBack)
        {
            calAppointments.SelectedDate = DateTime.Today;
            LoadAppointmentsForDate(DateTime.Today);
        }
    }

    private int GetTherapistId()
    {
        string query = "SELECT TherapistId FROM Therapists WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    /// <summary>
    /// Same payment-verified rule as Therapist/Requests.aspx.cs - a request the
    /// therapist can't act on yet (payment still awaiting Admin verification)
    /// shouldn't show up on their calendar either.
    /// </summary>
    private void LoadMonthAppointmentCounts(DateTime visibleMonth)
    {
        appointmentDayCounts.Clear();
        int therapistId = GetTherapistId();

        DateTime monthStart = new DateTime(visibleMonth.Year, visibleMonth.Month, 1);
        DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

        string query = @"SELECT a.AppointmentDate, COUNT(*) AS Total
                          FROM Appointments a
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.TherapistId = @TherapistId
                            AND pay.PaymentStatus = 'Success'
                            AND a.AppointmentDate BETWEEN @MonthStart AND @MonthEnd
                            AND a.Status NOT IN ('Cancelled','Rejected')
                          GROUP BY a.AppointmentDate";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@MonthStart", monthStart.ToString("yyyy-MM-dd")),
            new MySqlParameter("@MonthEnd", monthEnd.ToString("yyyy-MM-dd")));

        foreach (DataRow row in dt.Rows)
        {
            DateTime date = Convert.ToDateTime(row["AppointmentDate"]).Date;
            appointmentDayCounts[date] = Convert.ToInt32(row["Total"]);
        }
    }

    protected void calAppointments_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
    {
        if (appointmentDayCounts.ContainsKey(e.Day.Date))
        {
            int count = appointmentDayCounts[e.Day.Date];

            e.Cell.CssClass = string.IsNullOrEmpty(e.Cell.CssClass)
                ? "mc-day-has-appt"
                : e.Cell.CssClass + " mc-day-has-appt";

            e.Cell.ToolTip = count + " session" + (count == 1 ? "" : "s") + " on this day";
        }
    }

    protected void calAppointments_SelectionChanged(object sender, EventArgs e)
    {
        LoadAppointmentsForDate(calAppointments.SelectedDate);
    }

    protected void calAppointments_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
    {
        LoadMonthAppointmentCounts(e.NewDate);
    }

    private void LoadAppointmentsForDate(DateTime date)
    {
        litSelectedDate.Text = date.ToString("dd MMM yyyy");
        int therapistId = GetTherapistId();

        string query = @"SELECT u.FullName AS PatientName,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.Status
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users u ON u.UserId = p.UserId
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.TherapistId = @TherapistId AND a.AppointmentDate = @AppointmentDate
                            AND pay.PaymentStatus = 'Success'
                          ORDER BY a.AppointmentTime";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@AppointmentDate", date.ToString("yyyy-MM-dd")));

        if (dt.Rows.Count == 0)
        {
            lblNoneOnDate.Visible = true;
            rptDayAppointments.Visible = false;
        }
        else
        {
            lblNoneOnDate.Visible = false;
            rptDayAppointments.Visible = true;
            rptDayAppointments.DataSource = dt;
            rptDayAppointments.DataBind();
        }
    }
}
