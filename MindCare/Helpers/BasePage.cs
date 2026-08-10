using System;
using System.Web.UI;

/// <summary>
/// Every Patient dashboard page inherits from PatientBasePage instead of
/// System.Web.UI.Page. That way the "is this user logged in as a Patient?"
/// check happens automatically on every single page, instead of the
/// student having to copy-paste the same check into every Page_Load.
///
/// How to use in a code-behind file:
///     public partial class PatientDashboard : PatientBasePage { ... }
/// </summary>
public class PatientBasePage : Page
{
    protected int CurrentUserId;
    protected string CurrentUserName;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Session["UserId"] == null || Session["RoleId"].ToString() != "3")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        CurrentUserId = Convert.ToInt32(Session["UserId"]);
        CurrentUserName = Session["FullName"].ToString();
    }
}

public class TherapistBasePage : Page
{
    protected int CurrentUserId;
    protected string CurrentUserName;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Session["UserId"] == null || Session["RoleId"].ToString() != "2")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        CurrentUserId = Convert.ToInt32(Session["UserId"]);
        CurrentUserName = Session["FullName"].ToString();
    }
}

public class AdminBasePage : Page
{
    protected int CurrentUserId;
    protected string CurrentUserName;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Session["UserId"] == null || Session["RoleId"].ToString() != "1")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        CurrentUserId = Convert.ToInt32(Session["UserId"]);
        CurrentUserName = Session["FullName"].ToString();
    }
}
