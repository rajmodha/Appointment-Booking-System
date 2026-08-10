using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

/// <summary>
/// Inherits PatientBasePage instead of System.Web.UI.Page, so a logged-out
/// visitor (or a Therapist/Admin) is automatically redirected to Login before
/// this page's own code ever runs. See Helpers/BasePages.cs.
/// </summary>
public partial class BookAppointment : PatientBasePage
{
    private int therapistId;
    private decimal therapistFee;

    // We keep the chosen booking details in ViewState so they survive the postback
    // between "Proceed to Payment" and "I've Completed Payment" - NO Appointments or
    // Payments row is created until the patient actually submits a transaction ID.
    // This avoids leaving "ghost" Pending appointments in the database for people
    // who see the QR code but never actually pay.
    private string PendingDate
    {
        get { return ViewState["PendingDate"] as string; }
        set { ViewState["PendingDate"] = value; }
    }
    private string PendingTime
    {
        get { return ViewState["PendingTime"] as string; }
        set { ViewState["PendingTime"] = value; }
    }
    private string PendingConsultationType
    {
        get { return ViewState["PendingConsultationType"] as string; }
        set { ViewState["PendingConsultationType"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!int.TryParse(Request.QueryString["therapistId"], out therapistId))
        {
            ShowNotFound();
            return;
        }

        // This runs on EVERY request, including postbacks, because therapistFee
        // is a plain private field - it does NOT survive postback on its own
        // (only values stored in ViewState/Session do). Without this, clicking
        // "Proceed to Payment" would see therapistFee reset to 0.
        if (!LoadTherapistFee())
        {
            ShowNotFound();
            return;
        }

        if (!IsPostBack)
        {
            LoadTherapistSummary();
        }
    }

    private void ShowNotFound()
    {
        pnlSelectSlot.Visible = false;
        pnlNotFound.Visible = true;
    }

    /// <summary>
    /// Fetches just the fee (cheap query) so it's available every request,
    /// including postbacks. Returns false if the therapist doesn't exist /
    /// isn't approved.
    /// </summary>
    private bool LoadTherapistFee()
    {
        string query = @"SELECT Fees FROM Therapists
                          WHERE TherapistId = @TherapistId AND ApprovalStatus = 'Approved'";

        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@TherapistId", therapistId));

        if (result == null) return false;

        therapistFee = Convert.ToDecimal(result);
        return true;
    }

    /// <summary>
    /// Populates the visible page controls (name, fee display, consultation type
    /// options, date picker limits). Only needs to run once, on the initial GET -
    /// on postbacks the controls already retain their values automatically.
    /// </summary>
    private void LoadTherapistSummary()
    {
        string query = @"SELECT u.FullName, t.Fees, t.ConsultationType
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
        litTherapistName.Text = row["FullName"].ToString();
        litFee.Text = row["Fees"].ToString();

        // Only offer consultation types this therapist actually supports.
        string consultationType = row["ConsultationType"].ToString();
        ddlConsultationType.Items.Clear();
        if (consultationType == "Both")
        {
            ddlConsultationType.Items.Add("Online");
            ddlConsultationType.Items.Add("Offline");
        }
        else
        {
            ddlConsultationType.Items.Add(consultationType);
        }

        // Restrict the date picker to today .. today+30 days using HTML5 min/max
        txtDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
        txtDate.Attributes["max"] = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd");
    }

    protected void txtDate_TextChanged(object sender, EventArgs e)
    {
        LoadAvailableSlots();
    }

    /// <summary>
    /// Looks up the therapist's weekly availability for the chosen date's day-of-week,
    /// generates 1-hour slots across that window, then removes any slot that's already
    /// booked (Pending or Confirmed) for that exact date so two patients can't double-book.
    /// </summary>
    private void LoadAvailableSlots()
    {
        ddlTimeSlot.Items.Clear();

        DateTime chosenDate;
        if (!DateTime.TryParse(txtDate.Text, out chosenDate))
        {
            ddlTimeSlot.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select a date first --", ""));
            return;
        }

        string dayOfWeek = chosenDate.DayOfWeek.ToString(); // e.g. "Monday"

        string availabilityQuery = @"SELECT StartTime, EndTime FROM TherapistAvailability
                                      WHERE TherapistId = @TherapistId AND DayOfWeek = @DayOfWeek AND IsActive = 1";

        DataTable availabilityTable = DBHelper.ExecuteSelect(availabilityQuery,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@DayOfWeek", dayOfWeek));

        if (availabilityTable.Rows.Count == 0)
        {
            ddlTimeSlot.Items.Add(new System.Web.UI.WebControls.ListItem(
                "Therapist is not available on " + dayOfWeek, ""));
            return;
        }

        // Find times already booked on this exact date so we can exclude them.
        string bookedQuery = @"SELECT AppointmentTime FROM Appointments
                                WHERE TherapistId = @TherapistId AND AppointmentDate = @AppointmentDate
                                AND Status IN ('Pending','Confirmed')";

        DataTable bookedTable = DBHelper.ExecuteSelect(bookedQuery,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@AppointmentDate", chosenDate.ToString("yyyy-MM-dd")));

        HashSet<string> bookedTimes = new HashSet<string>();
        foreach (DataRow row in bookedTable.Rows)
            bookedTimes.Add(((TimeSpan)row["AppointmentTime"]).ToString(@"hh\:mm"));

        bool anySlotAdded = false;

        foreach (DataRow row in availabilityTable.Rows)
        {
            TimeSpan start = (TimeSpan)row["StartTime"];
            TimeSpan end = (TimeSpan)row["EndTime"];

            // Generate 1-hour slots, e.g. 10:00, 11:00, 12:00 ...
            for (TimeSpan slot = start; slot.Add(TimeSpan.FromHours(1)) <= end; slot = slot.Add(TimeSpan.FromHours(1)))
            {
                string slotKey = slot.ToString(@"hh\:mm");
                if (bookedTimes.Contains(slotKey)) continue;

                string displayText = DateTime.Today.Add(slot).ToString("h:mm tt");
                ddlTimeSlot.Items.Add(new System.Web.UI.WebControls.ListItem(displayText, slotKey));
                anySlotAdded = true;
            }
        }

        if (!anySlotAdded)
        {
            ddlTimeSlot.Items.Clear();
            ddlTimeSlot.Items.Add(new System.Web.UI.WebControls.ListItem("No open slots on this date - try another day", ""));
        }
    }

    protected void btnProceedToPayment_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlTimeSlot.SelectedValue))
        {
            lblSlotError.Text = "Please choose a valid date and time slot.";
            return;
        }

        DateTime chosenDate;
        if (!DateTime.TryParse(txtDate.Text, out chosenDate))
        {
            lblSlotError.Text = "Please choose a valid date.";
            return;
        }

        // Just double-check the patient profile exists before letting them go
        // any further - nothing is written to the database yet at this point.
        string patientIdQuery = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object patientIdResult = DBHelper.ExecuteScalar(patientIdQuery, new MySqlParameter("@UserId", CurrentUserId));

        if (patientIdResult == null)
        {
            lblSlotError.Text = "Your patient profile could not be found. Please contact support.";
            return;
        }

        // Remember the chosen slot so the Confirm Payment step (below) can use it -
        // the Appointments/Payments rows are only created once payment is actually
        // confirmed, not here. This is what stops an abandoned QR screen (someone
        // who never pays) from leaving a ghost "Pending" appointment in the database.
        PendingDate = chosenDate.ToString("yyyy-MM-dd");
        PendingTime = ddlTimeSlot.SelectedValue;
        PendingConsultationType = ddlConsultationType.SelectedValue;

        // Move to the payment step and generate the QR code for this exact amount.
        // The QR's "note" field is just a human-readable label for the UPI app -
        // it can't reference an AppointmentId yet since none exists until payment
        // is confirmed, so we use the patient's UserId + a timestamp instead.
        litPayTherapistName.Text = litTherapistName.Text;
        litPayDateTime.Text = chosenDate.ToString("dd MMM yyyy") + " at " + ddlTimeSlot.SelectedItem.Text;
        litPayAmount.Text = therapistFee.ToString("0.00");

        string qrNote = "MindCare-U" + CurrentUserId + "-" + DateTime.Now.Ticks;
        imgUpiQr.ImageUrl = ResolveUrl("~/GenerateQR.ashx?amount=" + therapistFee.ToString("0.00") +
                                        "&note=" + Server.UrlEncode(qrNote));

        pnlSelectSlot.Visible = false;
        pnlPayment.Visible = true;
    }

    protected void btnConfirmPayment_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        if (string.IsNullOrEmpty(PendingDate) || string.IsNullOrEmpty(PendingTime))
        {
            lblPaymentError.Text = "Something went wrong - please start the booking again.";
            return;
        }

        string patientIdQuery = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object patientIdResult = DBHelper.ExecuteScalar(patientIdQuery, new MySqlParameter("@UserId", CurrentUserId));

        if (patientIdResult == null)
        {
            lblPaymentError.Text = "Your patient profile could not be found. Please contact support.";
            return;
        }
        int patientId = Convert.ToInt32(patientIdResult);

        // Re-check the slot wasn't taken by someone else while this patient was
        // sitting on the QR screen (a real, if narrow, race condition).
        string clashQuery = @"SELECT COUNT(*) FROM Appointments
                               WHERE TherapistId = @TherapistId AND AppointmentDate = @AppointmentDate
                                 AND AppointmentTime = @AppointmentTime AND Status IN ('Pending','Confirmed')";

        object clashCount = DBHelper.ExecuteScalar(clashQuery,
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@AppointmentDate", PendingDate),
            new MySqlParameter("@AppointmentTime", PendingTime));

        if (Convert.ToInt32(clashCount) > 0)
        {
            lblPaymentError.Text = "Sorry, this slot was just booked by someone else. Please go back and choose another time.";
            return;
        }

        // NOW - and only now, with a real transaction reference in hand - do we
        // create the Appointment (Pending, awaiting therapist acceptance) and its
        // matching Payment (Success) row together.
        string insertAppointment = @"INSERT INTO Appointments
                                      (PatientId, TherapistId, AppointmentDate, AppointmentTime, ConsultationType, Status, Amount)
                                      VALUES (@PatientId, @TherapistId, @AppointmentDate, @AppointmentTime, @ConsultationType, 'Pending', @Amount)";

        long appointmentId = DBHelper.ExecuteInsertAndGetId(insertAppointment,
            new MySqlParameter("@PatientId", patientId),
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@AppointmentDate", PendingDate),
            new MySqlParameter("@AppointmentTime", PendingTime),
            new MySqlParameter("@ConsultationType", PendingConsultationType),
            new MySqlParameter("@Amount", therapistFee));

        string insertPayment = @"INSERT INTO Payments (AppointmentId, Amount, PaymentStatus, UpiTransactionRef)
                                  VALUES (@AppointmentId, @Amount, 'Pending', @Ref)";

        DBHelper.ExecuteNonQuery(insertPayment,
            new MySqlParameter("@AppointmentId", appointmentId),
            new MySqlParameter("@Amount", therapistFee),
            new MySqlParameter("@Ref", txtUpiRef.Text.Trim()));

        // NOTE: we do NOT send the confirmation email here, and the therapist does
        // NOT see this appointment yet either (see Therapist/Requests.aspx.cs's
        // WHERE clause). Both of those only happen once Admin has manually checked
        // the UPI transaction reference and verified the payment actually went
        // through - see Admin/VerifyPayments.aspx.

        pnlPayment.Visible = false;
        pnlConfirmed.Visible = true;
    }

}
