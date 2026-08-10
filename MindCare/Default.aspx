    <%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ============ HERO ============ -->
    <div class="mc-hero">
        <h1>Your journey to better mental health starts here</h1>

        <p>
            MindCare connects you with verified, licensed therapists for online and
            offline sessions — book in minutes, in a space designed to feel calm and safe.
        </p>

        <a href="~/FindTherapist.aspx" runat="server" class="mc-btn">
            Find a Therapist
        </a>

        <a href="~/Register.aspx" runat="server" class="mc-btn mc-btn-outline" style="margin-left:12px;">
            Get Started
        </a>
    </div>


    <!-- ============ WHY THERAPY MATTERS ============ -->
    <div style="max-width:800px; margin:60px auto; padding:0 20px; text-align:center;">

        <h2 style="color:var(--mc-primary-dark);">
            Why Therapy Matters
        </h2>

        <p style="color:var(--mc-muted); font-size:17px; line-height:1.8;">
            Life doesn't come with a manual — and everyone hits moments where things
            feel harder to carry alone. Therapy is a collaborative process with a
            trained professional, giving you space to understand your thoughts and
            feelings, build practical coping tools, and work through challenges at
            your own pace. Reaching out isn't a sign of weakness — it's one of the
            most proactive things you can do for yourself.
        </p>

    </div>


    <!-- ============ BENEFITS OF THERAPY ============ -->
    <div style="background:linear-gradient(135deg, var(--mc-lavender), var(--mc-mint)); padding:60px 20px;">

        <div style="max-width:1100px; margin:0 auto;">

            <h2 style="text-align:center; color:var(--mc-primary-dark);">
                The Benefits of Therapy
            </h2>

            <div style="display:flex; gap:20px; flex-wrap:wrap; margin-top:30px;">

                <!-- Benefit 1 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">🧘</div>

                    <h4 style="margin:8px 0 4px 0;">
                        Better Emotional Regulation
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Learn practical tools to manage stress, anxiety, and difficult
                        emotions in healthier ways.
                    </p>
                </div>


                <!-- Benefit 2 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">🤝</div>

                    <h4 style="margin:8px 0 4px 0;">
                        Stronger Relationships
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Improve communication and recognize recurring patterns in how
                        you connect with others.
                    </p>
                </div>


                <!-- Benefit 3 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">🔎</div>

                    <h4 style="margin:8px 0 4px 0;">
                        Increased Self-Awareness
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Understand your own thought patterns and behaviors to make more
                        intentional choices.
                    </p>
                </div>


                <!-- Benefit 4 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">🌱</div>

                    <h4 style="margin:8px 0 4px 0;">
                        Healthier Coping Skills
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Replace unhelpful habits with strategies that genuinely support
                        your wellbeing long-term.
                    </p>
                </div>


                <!-- Benefit 5 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">🔒</div>

                    <h4 style="margin:8px 0 4px 0;">
                        A Safe, Judgment-Free Space
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Talk openly with someone trained to listen without judgment,
                        in complete confidence.
                    </p>
                </div>


                <!-- Benefit 6 -->
                <div class="mc-card" style="flex:1; min-width:280px;">
                    <div style="font-size:26px;">💪</div>

                    <h4 style="margin:8px 0 4px 0;">
                        Long-Term Resilience
                    </h4>

                    <p style="color:var(--mc-muted); font-size:14px; margin:0;">
                        Build skills that help you navigate future challenges, not just
                        the one in front of you today.
                    </p>
                </div>

            </div>
        </div>
    </div>


    <!-- ============ IS THERAPY RIGHT FOR YOU ============ -->
    <div style="max-width:800px; margin:60px auto; padding:0 20px;">

        <h2 style="text-align:center; color:var(--mc-primary-dark);">
            Is Therapy Right for You?
        </h2>

        <p style="color:var(--mc-muted); text-align:center; font-size:16px;">
            People come to therapy for all kinds of reasons — you don't need to be
            in crisis to benefit. You might consider reaching out if you're noticing:
        </p>


        <div class="mc-card" style="margin-top:24px;">

            <ul style="margin:0; padding-left:20px; color:var(--mc-text); line-height:2;">

                <li>
                    Persistent feelings of stress, worry, or low mood
                </li>

                <li>
                    Changes in sleep, appetite, or energy
                </li>

                <li>
                    Difficulty in relationships, at work, or at home
                </li>

                <li>
                    A major life transition, loss, or big decision
                </li>

                <li>
                    Simply wanting to understand yourself better
                </li>

            </ul>

        </div>


        <p style="color:var(--mc-muted); text-align:center; margin-top:20px;">
            Many people find it helpful just to have a space to think out loud
            with someone trained to listen.
        </p>


        <p style="background:#FFF3CD; color:#8A6D00; padding:14px 20px;
                  border-radius:12px; font-size:14px; text-align:center; margin-top:24px;">

            ⚠️ If you're in crisis or experiencing thoughts of self-harm, please
            contact your local emergency services or a crisis helpline immediately —
            MindCare is not an emergency service.

        </p>

    </div>


    <!-- ============ WHY CHOOSE MINDCARE ============ -->
    <div style="max-width:1100px; margin:60px auto; padding:0 20px;">

        <h2 style="text-align:center; color:var(--mc-primary-dark);">
            Why Choose MindCare
        </h2>


        <div style="display:flex; gap:24px; flex-wrap:wrap; margin-top:30px;">

            <!-- Verified Therapists -->
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">

                <h3>
                    Verified Therapists
                </h3>

                <p style="color:var(--mc-muted);">
                    Every therapist is manually reviewed and approved by our admin
                    team before they can accept patients.
                </p>

            </div>


            <!-- Easy Booking -->
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">

                <h3>
                    Easy Booking
                </h3>

                <p style="color:var(--mc-muted);">
                    Filter by specialization, language, location and fees, then book
                    a slot that works for you.
                </p>

            </div>


            <!-- Secure Payment -->
            <div class="mc-card" style="flex:1; min-width:250px; text-align:center;">

                <h3>
                    Secure UPI Payment
                </h3>

                <p style="color:var(--mc-muted);">
                    Pay instantly with a dynamically generated UPI QR code — the exact
                    amount, every time, no surprises.
                </p>

            </div>

        </div>
    </div>


    <!-- ============ MEET OUR THERAPISTS ============ -->
    <div style="max-width:1100px; margin:60px auto; padding:0 20px;">

        <h2 style="text-align:center; color:var(--mc-primary-dark);">
            Meet Some of Our Therapists
        </h2>


        <div style="display:flex; gap:20px; flex-wrap:wrap; margin-top:30px;">

            <asp:Repeater
                ID="rptFeaturedTherapists"
                runat="server"
                OnItemDataBound="rptFeaturedTherapists_ItemDataBound">

                <ItemTemplate>

                    <div class="mc-card mc-therapist-card"
                         style="flex:1; min-width:280px;">

                        <asp:Image
                            ID="imgTherapist"
                            runat="server"
                            AlternateText="" />

                        <div>

                            <h4 style="margin:0;">
                                <%# Eval("FullName") %>
                            </h4>

                            <p style="margin:4px 0; color:var(--mc-muted);">
                                <%# Eval("Specialization") %>
                            </p>

                            <p style="margin:0; font-weight:600; color:var(--mc-primary);">
                                ₹<%# Eval("Fees") %> / session
                            </p>

                        </div>

                    </div>

                </ItemTemplate>

            </asp:Repeater>

        </div>

    </div>

</asp:Content>