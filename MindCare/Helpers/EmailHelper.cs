using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

/// <summary>
/// EmailHelper sends emails using the SMTP settings stored in Web.config.
/// Used for: appointment confirmations and password reset links.
/// For a student project, Gmail SMTP with an "App Password" works well.
/// </summary>
public class EmailHelper
{
    public static void SendEmail(string toEmail, string subject, string bodyHtml)
    {
        try
        {
            string fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
            string fromPassword = ConfigurationManager.AppSettings["SmtpFromPassword"];
            string host = ConfigurationManager.AppSettings["SmtpHost"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, "MindCare");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = bodyHtml;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(host, port);
            smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        catch (Exception ex)
        {
            // In a student project we log the error instead of crashing the
            // whole booking flow just because the email failed to send.
            System.Diagnostics.Debug.WriteLine("Email send failed: " + ex.Message);
        }
    }

    /// <summary>
    /// Sent by Admin/VerifyPayments.aspx.cs right after the payment is verified.
    /// This is NOT the final confirmation - the therapist still has to accept the
    /// request. Wording matters here: saying "confirmed" at this stage would be
    /// misleading, since the therapist could still reject it afterwards.
    /// </summary>
    public static string BuildPaymentVerifiedBody(string patientName, string therapistName,
        DateTime date, string time, string consultationType)
    {
        return "<h2>Payment Verified - MindCare</h2>" +
               "<p>Hi " + patientName + ",</p>" +
               "<p>We've verified your payment for your session with <b>" + therapistName + "</b>.</p>" +
               "<p><b>Date:</b> " + date.ToString("dd MMM yyyy") + "<br/>" +
               "<b>Time:</b> " + time + "<br/>" +
               "<b>Mode:</b> " + consultationType + "</p>" +
               "<p>Your request has now been sent to the therapist. You'll receive another " +
               "email once they've confirmed the session - you can also track its status " +
               "anytime from \"My Appointments\".</p>" +
               "<p>Thank you for choosing MindCare.</p>";
    }

    /// <summary>
    /// Sent by Admin/VerifyPayments.aspx.cs to the THERAPIST (not the patient),
    /// right alongside BuildPaymentVerifiedBody above - this is what actually
    /// nudges them to go accept/reject the request, since Requests.aspx only
    /// shows payment-verified appointments and they'd otherwise have no way
    /// of knowing a new one just became visible without checking manually.
    /// </summary>
    public static string BuildTherapistNewRequestBody(string therapistName, string patientName,
        DateTime date, string time, string consultationType)
    {
        return "<h2>New Appointment Request - MindCare</h2>" +
               "<p>Hi Dr. " + therapistName + ",</p>" +
               "<p>You have a new appointment request from <b>" + patientName + "</b>, and their " +
               "payment has been verified.</p>" +
               "<p><b>Date:</b> " + date.ToString("dd MMM yyyy") + "<br/>" +
               "<b>Time:</b> " + time + "<br/>" +
               "<b>Mode:</b> " + consultationType + "</p>" +
               "<p>Please log in and go to <b>Appointment Requests</b> to Accept or Reject this " +
               "session" + (consultationType == "Online" ? ", and add a meeting link if you accept" : "") + ".</p>" +
               "<p>Thank you for being a MindCare therapist.</p>";
    }

    /// <summary>
    /// Sent by Admin/VerifyPayments.aspx.cs when a payment's transaction
    /// reference couldn't be verified - lets the patient know why their
    /// appointment disappeared instead of leaving them guessing.
    /// </summary>
    public static string BuildPaymentRejectedBody(string patientName, string therapistName,
        DateTime date, string time)
    {
        return "<h2>Payment Could Not Be Verified - MindCare</h2>" +
               "<p>Hi " + patientName + ",</p>" +
               "<p>We were unable to verify the payment for your requested session with " +
               "<b>" + therapistName + "</b> on " + date.ToString("dd MMM yyyy") + " at " + time + ".</p>" +
               "<p>As a result, this appointment has been cancelled and the slot has been " +
               "released. This can happen if the transaction reference entered didn't match " +
               "a real payment, or if the payment itself didn't go through on your UPI app's end.</p>" +
               "<p>You're welcome to try booking again - if you believe this was a mistake, " +
               "please reach out through our Contact page with your transaction details.</p>" +
               "<p>We're sorry for the inconvenience.</p>";
    }

    /// <summary>
    /// Sent by Patient/MyAppointments.aspx.cs to the THERAPIST when a patient
    /// cancels their own appointment (as opposed to the therapist rejecting or
    /// cancelling it themselves - see Requests.aspx.cs for that side). Purely
    /// informational - no action needed from the therapist beyond knowing
    /// their schedule just freed up.
    /// </summary>
    public static string BuildTherapistPatientCancelledBody(string therapistName, string patientName,
        DateTime date, string time, bool wasConfirmed)
    {
        return "<h2>Session Cancelled by Patient - MindCare</h2>" +
               "<p>Hi Dr. " + therapistName + ",</p>" +
               "<p><b>" + patientName + "</b> has cancelled their " +
               (wasConfirmed ? "confirmed" : "requested") + " session with you on " +
               date.ToString("dd MMM yyyy") + " at " + time + ".</p>" +
               "<p>No action is needed on your end - this slot is now free.</p>";
    }

    /// <summary>
    /// Sent by Patient/MyAppointments.aspx.cs to every Admin account when a
    /// patient cancels their own appointment. Unlike the therapist-initiated
    /// case (BuildAdminRefundNoticeBody), a patient can cancel BEFORE their
    /// payment has even been verified by Admin - so this only tells Admin a
    /// refund is actually owed if paymentWasVerified is true. If the payment
    /// was still sitting unverified, no money was ever counted as collected
    /// on MindCare's side, so there's nothing to refund from our system's
    /// perspective (whatever happened with the patient's own UPI app is
    /// between them and Admin to sort out via Contact if needed).
    /// </summary>
    public static string BuildAdminPatientCancelNoticeBody(string patientName, string patientEmail, string therapistName,
        DateTime date, string time, decimal amount, string upiTransactionRef, bool paymentWasVerified)
    {
        if (paymentWasVerified)
        {
            return "<h2>Refund Needed - Patient Cancelled - MindCare</h2>" +
                   "<p><b>" + patientName + "</b> (" + patientEmail + ") has cancelled their own " +
                   "appointment with <b>" + therapistName + "</b>, for which payment was already " +
                   "verified. Please process a manual refund.</p>" +
                   "<p><b>Session Date/Time:</b> " + date.ToString("dd MMM yyyy") + " at " + time + "<br/>" +
                   "<b>Amount to Refund:</b> ₹" + amount.ToString("0.00") + "<br/>" +
                   "<b>UPI Transaction Reference:</b> " + upiTransactionRef + "</p>" +
                   "<p>Use the transaction reference above to locate the original payment in your " +
                   "UPI app and issue a refund, then mark it as refunded in the Admin Refunds page.</p>";
        }
        else
        {
            return "<h2>Appointment Cancelled by Patient - MindCare</h2>" +
                   "<p><b>" + patientName + "</b> (" + patientEmail + ") has cancelled their appointment " +
                   "with <b>" + therapistName + "</b> before their payment was verified.</p>" +
                   "<p><b>Session Date/Time:</b> " + date.ToString("dd MMM yyyy") + " at " + time + "</p>" +
                   "<p>No refund action is needed from MindCare's side, since this payment was never " +
                   "marked verified. No further action required.</p>";
        }
    }

    /// <summary>
    /// Sent by Therapist/Requests.aspx.cs when the therapist clicks "Accept" -
    /// this is the actual, final confirmation that the session is happening.
    /// </summary>
    public static string BuildAppointmentConfirmationBody(string patientName, string therapistName,
        DateTime date, string time, string consultationType)
    {
        return "<h2>Appointment Confirmed - MindCare</h2>" +
               "<p>Hi " + patientName + ",</p>" +
               "<p>Good news - <b>" + therapistName + "</b> has confirmed your session.</p>" +
               "<p><b>Date:</b> " + date.ToString("dd MMM yyyy") + "<br/>" +
               "<b>Time:</b> " + time + "<br/>" +
               "<b>Mode:</b> " + consultationType + "</p>" +
               "<p>Thank you for choosing MindCare.</p>";
    }

    /// <summary>
    /// Sent by Therapist/Requests.aspx.cs to the PATIENT when a therapist
    /// rejects a still-Pending request, or cancels an already-Confirmed one.
    /// By the time a therapist can see a request at all, Payments.PaymentStatus
    /// is already 'Success' (see Requests.aspx.cs's WHERE clause), so real
    /// money was collected and needs refunding - this email tells the patient
    /// that's happening, without them needing to chase it themselves.
    /// </summary>
    public static string BuildTherapistDeclinedBody(string patientName, string therapistName,
        DateTime date, string time, bool wasAlreadyConfirmed)
    {
        string situation = wasAlreadyConfirmed
            ? "had to cancel your already-confirmed session"
            : "was unable to accept your session request";

        return "<h2>Your Session Was " + (wasAlreadyConfirmed ? "Cancelled" : "Declined") + " - MindCare</h2>" +
               "<p>Hi " + patientName + ",</p>" +
               "<p><b>" + therapistName + "</b> " + situation + " on " +
               date.ToString("dd MMM yyyy") + " at " + time + ".</p>" +
               "<p>Since your payment for this session was already verified, a refund will be " +
               "processed to your original UPI account. Our admin team has been notified with " +
               "your transaction details and will process it shortly.</p>" +
               "<p>We're sorry for the inconvenience - you're welcome to book another therapist " +
               "or a different time slot anytime.</p>";
    }

    /// <summary>
    /// Sent by Therapist/Requests.aspx.cs to EVERY Admin account when a therapist
    /// rejects or cancels a payment-verified appointment. This is a manual-refund
    /// system (no payment gateway integration), so Admin needs the UPI transaction
    /// reference in hand to actually find and refund that specific payment.
    /// </summary>
    public static string BuildAdminRefundNoticeBody(string patientName, string patientEmail, string therapistName,
        DateTime date, string time, decimal amount, string upiTransactionRef, bool wasAlreadyConfirmed)
    {
        return "<h2>Refund Needed - MindCare</h2>" +
               "<p><b>" + therapistName + "</b> has " +
               (wasAlreadyConfirmed ? "cancelled an already-confirmed session" : "rejected a session request") +
               " for which payment was already verified. Please process a manual refund.</p>" +
               "<p><b>Patient:</b> " + patientName + " (" + patientEmail + ")<br/>" +
               "<b>Therapist:</b> " + therapistName + "<br/>" +
               "<b>Session Date/Time:</b> " + date.ToString("dd MMM yyyy") + " at " + time + "<br/>" +
               "<b>Amount to Refund:</b> ₹" + amount.ToString("0.00") + "<br/>" +
               "<b>UPI Transaction Reference:</b> " + upiTransactionRef + "</p>" +
               "<p>Use the transaction reference above to locate the original payment in your UPI " +
               "app and issue a refund to the patient's account.</p>";
    }

    /// <summary>
    /// Sent by Admin/Refunds.aspx.cs the moment Admin marks a payment as
    /// refunded - confirms it actually happened, with the refund's OWN
    /// transaction reference (distinct from the original payment's ref) so
    /// the patient has something concrete to check against their UPI app.
    /// </summary>
    public static string BuildRefundProcessedBody(string patientName, string therapistName,
        DateTime date, string time, decimal amount, string refundTransactionRef)
    {
        return "<h2>Refund Processed - MindCare</h2>" +
               "<p>Hi " + patientName + ",</p>" +
               "<p>Your refund of <b>₹" + amount.ToString("0.00") + "</b> for the session with " +
               "<b>" + therapistName + "</b> on " + date.ToString("dd MMM yyyy") + " at " + time +
               " has been processed.</p>" +
               "<p><b>Refund Transaction Reference:</b> " + refundTransactionRef + "</p>" +
               "<p>Please allow a few business days for it to reflect in your bank/UPI account, " +
               "depending on your bank. If you don't see it after that, please reach out through " +
               "our Contact page with this reference number.</p>" +
               "<p>We're sorry the session didn't go through, and hope to see you back on MindCare.</p>";
    }
}
