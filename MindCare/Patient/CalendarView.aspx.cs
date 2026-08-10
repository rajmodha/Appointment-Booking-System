using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Patient_CalendarView : PatientBasePage
{
    // Maps each date that has at least one appointment to how many appointments
    // fall on it - loaded fresh for whichever month is currently visible, since
    // DayRender fires for every visible cell on every postback (including when
    // the patient clicks the "next month" arrow).
    private Dictionary<DateTime, int> appointmentDayCounts = new Dictionary<DateTime, int>();

    protected void Page_Load(object sender, EventArgs e)
    {
        PatientSidebar1.ActivePage = "Calendar";

        // IMPORTANT: Calendar.VisibleDate defaults to DateTime.MinValue (0001-01-01)
        // until the user actually navigates months with the < / > arrows - even
        // though the control still visually displays the current month by falling
        // back to TodaysDate internally for rendering. Reading VisibleDate directly
        // without this check silently queries the wrong month (this was the actual
        // bug - the debug output showed VisibleDate=0001-01-01, which is why the
        // month-range query always found 0 rows despite the data being correct).
        DateTime effectiveMonth = calAppointments.VisibleDate == DateTime.MinValue
            ? calAppointments.TodaysDate
            : calAppointments.VisibleDate;

        LoadMonthAppointmentCounts(effectiveMonth);

        if (!IsPostBack)
        {
            // Show today's appointments (if any) by default.
            calAppointments.SelectedDate = DateTime.Today;
            LoadAppointmentsForDate(DateTime.Today);
        }
    }

    private int GetPatientId()
    {
        string query = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    /// <summary>
    /// Pulls just the dates (not full details) of every appointment in the
    /// given month, so DayRender can cheaply check "does this date have
    /// anything?" without a database round-trip per day cell.
    /// </summary>
    private void LoadMonthAppointmentCounts(DateTime visibleMonth)
    {
        appointmentDayCounts.Clear();
        int patientId = GetPatientId();

        DateTime monthStart = new DateTime(visibleMonth.Year, visibleMonth.Month, 1);
        DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

        string query = @"SELECT AppointmentDate, COUNT(*) AS Total
                          FROM Appointments
                          WHERE PatientId = @PatientId
                            AND AppointmentDate BETWEEN @MonthStart AND @MonthEnd
                            AND Status NOT IN ('Cancelled','Rejected')
                          GROUP BY AppointmentDate";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@PatientId", patientId),
            new MySqlParameter("@MonthStart", monthStart.ToString("yyyy-MM-dd")),
            new MySqlParameter("@MonthEnd", monthEnd.ToString("yyyy-MM-dd")));

        foreach (DataRow row in dt.Rows)
        {
            DateTime date = Convert.ToDateTime(row["AppointmentDate"]).Date;
            appointmentDayCounts[date] = Convert.ToInt32(row["Total"]);
        }
    }

    /// <summary>
    /// Adds a small dot + count under any date that has appointments. This
    /// fires once per visible day cell (including the greyed-out days that
    /// spill into the previous/next month), so keep it cheap - the dictionary
    /// lookup here is O(1), no database call per cell.
    /// </summary>
    protected void calAppointments_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
    {
        if (appointmentDayCounts.ContainsKey(e.Day.Date))
        {
            int count = appointmentDayCounts[e.Day.Date];

            // BackColor is an inline style attribute set directly on the <td> -
            // this cannot fail to render regardless of any Text/Controls
            // rendering quirk or stylesheet caching, so it's our ground truth
            // for whether DayRender is even reaching this branch for this date.

            e.Cell.CssClass = string.IsNullOrEmpty(e.Cell.CssClass)
                ? "mc-day-has-appt"
                : e.Cell.CssClass + " mc-day-has-appt";

            e.Cell.ToolTip = count + " appointment" + (count == 1 ? "" : "s") + " on this day";
        }
    }

    protected void calAppointments_SelectionChanged(object sender, EventArgs e)
    {
        LoadAppointmentsForDate(calAppointments.SelectedDate);
    }

    /// <summary>
    /// Re-fetching the month's appointment dots is necessary here too, because
    /// switching months is itself a postback, and DayRender needs fresh data
    /// for whatever month the calendar just navigated to.
    /// </summary>
    protected void calAppointments_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
    {
        LoadMonthAppointmentCounts(e.NewDate);
    }

    private void LoadAppointmentsForDate(DateTime date)
    {
        litSelectedDate.Text = date.ToString("dd MMM yyyy");
        int patientId = GetPatientId();

        string query = @"SELECT u.FullName AS TherapistName,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.Status
                          FROM Appointments a
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE a.PatientId = @PatientId AND a.AppointmentDate = @AppointmentDate
                          ORDER BY a.AppointmentTime";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@PatientId", patientId),
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
