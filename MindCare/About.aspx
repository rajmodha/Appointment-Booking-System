<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ============ HERO ============ -->
    <div class="mc-hero">
        <h1>Mental health support, made simple</h1>
        <p>MindCare connects you with verified, licensed therapists for online and offline
           sessions — because taking care of your mind should feel as easy as booking
           any other appointment.</p>
    </div>

    <!-- ============ MISSION ============ -->
    <div style="max-width:800px; margin:60px auto; padding:0 20px; text-align:center;">
        <h2 style="color:var(--mc-primary-dark);">Our Mission</h2>
        <p style="color:var(--mc-muted); font-size:17px; line-height:1.7;">
            Finding the right therapist shouldn't be harder than the problem that brought you
            here in the first place. We built MindCare to remove the friction — from
            searching by what actually matters to you (language, budget, specialization,
            location), to booking a slot, to paying, to following up — so you can spend less
            time navigating a system and more time getting better.
        </p>
    </div>

    <!-- ============ LIVE STATS ============ -->
    <div style="background:linear-gradient(135deg, var(--mc-lavender), var(--mc-mint)); padding:50px 20px;">
        <div style="max-width:900px; margin:0 auto; display:flex; gap:24px; flex-wrap:wrap; text-align:center;">
            <div style="flex:1; min-width:180px;">
                <div style="font-size:36px; font-weight:700; color:var(--mc-primary-dark);"><asp:Literal ID="litTherapistCount" runat="server" /></div>
                <div style="color:var(--mc-muted);">Verified Therapists</div>
            </div>
            <div style="flex:1; min-width:180px;">
                <div style="font-size:36px; font-weight:700; color:var(--mc-primary-dark);"><asp:Literal ID="litSessionsCompleted" runat="server" /></div>
                <div style="color:var(--mc-muted);">Sessions Completed</div>
            </div>
            <div style="flex:1; min-width:180px;">
                <div style="font-size:36px; font-weight:700; color:var(--mc-primary-dark);"><asp:Literal ID="litAvgRating" runat="server" /></div>
                <div style="color:var(--mc-muted);">Average Patient Rating</div>
            </div>
            <div style="flex:1; min-width:180px;">
                <div style="font-size:36px; font-weight:700; color:var(--mc-primary-dark);"><asp:Literal ID="litCategoryCount" runat="server" /></div>
                <div style="color:var(--mc-muted);">Areas of Specialization</div>
            </div>
        </div>
    </div>

    <!-- ============ HOW IT WORKS ============ -->
    <div style="max-width:1000px; margin:60px auto; padding:0 20px;">
        <h2 style="text-align:center; color:var(--mc-primary-dark);">How MindCare works</h2>
        <div style="display:flex; gap:24px; flex-wrap:wrap; margin-top:30px;">
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">
                <div style="font-size:32px;">🔍</div>
                <h3>1. Find your fit</h3>
                <p style="color:var(--mc-muted);">Filter therapists by specialization, language, location, and budget until you find someone right for you.</p>
            </div>
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">
                <div style="font-size:32px;">📅</div>
                <h3>2. Book &amp; pay securely</h3>
                <p style="color:var(--mc-muted);">Pick an open slot and pay instantly with a UPI QR code — the exact amount, every time, no surprises.</p>
            </div>
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">
                <div style="font-size:32px;">💜</div>
                <h3>3. Attend &amp; grow</h3>
                <p style="color:var(--mc-muted);">Join your session online or in person, and leave feedback afterward to help others find the right fit too.</p>
            </div>
        </div>
    </div>

    <!-- ============ VALUES ============ -->
    <div style="max-width:1000px; margin:60px auto; padding:0 20px;">
        <h2 style="text-align:center; color:var(--mc-primary-dark);">What we stand for</h2>
        <div style="display:flex; gap:24px; flex-wrap:wrap; margin-top:30px;">
            <div class="mc-card" style="flex:1; min-width:250px;">
                <h3 style="margin-top:0;">Verified, always</h3>
                <p style="color:var(--mc-muted);">Every therapist on MindCare is reviewed and approved by our team before they can accept a single patient.</p>
            </div>
            <div class="mc-card" style="flex:1; min-width:250px;">
                <h3 style="margin-top:0;">Privacy first</h3>
                <p style="color:var(--mc-muted);">Your bookings, payments, and conversations with your therapist stay between you and them.</p>
            </div>
            <div class="mc-card" style="flex:1; min-width:250px;">
                <h3 style="margin-top:0;">No hidden fees</h3>
                <p style="color:var(--mc-muted);">The price you see on a therapist's profile is exactly what you pay - nothing added at checkout.</p>
            </div>
        </div>
    </div>

    <!-- ============ CTA ============ -->
    <div style="background:var(--mc-lavender); padding:60px 20px; text-align:center;">
        <h2 style="color:var(--mc-primary-dark);">Ready to talk to someone?</h2>
        <p style="color:var(--mc-muted); margin-bottom:24px;">Your first step doesn't have to be a big one.</p>
        <a href="~/FindTherapist.aspx" runat="server" class="mc-btn">Find a Therapist</a>
        <a href="~/Register.aspx" runat="server" class="mc-btn mc-btn-outline" style="margin-left:12px;">Create an Account</a>
    </div>

</asp:Content>
