using System;
using System.Web;
using System.Configuration;
using QRCoder;

/// <summary>
/// GenerateQR.ashx is a "Generic Handler" - unlike a .aspx page it has no HTML,
/// it just outputs raw bytes. We use it to generate a UPI payment QR code image
/// on the fly, with the exact appointment amount embedded in it.
///
/// Call it like:  GenerateQR.ashx?amount=800.00&note=Appt12
///
/// Requires the QRCoder NuGet package (Tools > NuGet Package Manager > search "QRCoder").
/// </summary>
public class GenerateQR : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Read the amount + a short note (e.g. appointment id) from the query string.
        string amount = context.Request.QueryString["amount"];
        string note = context.Request.QueryString["note"] ?? "MindCare Appointment";

        decimal parsedAmount;
        if (string.IsNullOrWhiteSpace(amount) || !decimal.TryParse(amount, out parsedAmount))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            context.Response.Write("Missing or invalid amount.");
            return;
        }

        try
        {
            // UPI ID and payee name come from Web.config so they're easy to change
            // without touching code. Fall back to safe defaults if the appSettings
            // keys are missing, so a Web.config that wasn't fully merged doesn't
            // crash the whole handler with a blank 500 error.
            string upiId = ConfigurationManager.AppSettings["UpiId"];
            string payeeName = ConfigurationManager.AppSettings["UpiPayeeName"];

            if (string.IsNullOrWhiteSpace(upiId)) upiId = "mindcare@upi";
            if (string.IsNullOrWhiteSpace(payeeName)) payeeName = "MindCare";

            // This is the standard UPI deep-link format. Any UPI app (GPay, PhonePe,
            // Paytm, etc.) knows how to read this exact string from a QR code.
            // pa = payee address (UPI ID), pn = payee name, am = amount, cu = currency
            string upiPaymentString = string.Format(
                "upi://pay?pa={0}&pn={1}&am={2}&cu=INR&tn={3}",
                Uri.EscapeDataString(upiId),
                Uri.EscapeDataString(payeeName),
                parsedAmount.ToString("0.00"),
                Uri.EscapeDataString(note));

            // Generate the QR code as a PNG image using QRCoder.
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(upiPaymentString, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(10); // 10 = pixels per QR "module"

                context.Response.ContentType = "image/png";
                context.Response.BinaryWrite(qrCodeImage);
            }
        }
        catch (Exception ex)
        {
            // Surface the real error as plain text instead of letting it bubble up
            // as a generic, unhelpful 500 - much easier to debug during development.
            // (In a real production app you'd log this instead of returning it to
            // the browser, but for a student project seeing the cause directly saves
            // a lot of guesswork.)
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            context.Response.Write("QR generation failed: " + ex.Message);
        }
    }

    // Required by IHttpHandler. False means a new instance is created per
    // request, which is the simplest and safest option here.
    public bool IsReusable
    {
        get { return false; }
    }
}
