// Owner: Reyes | Type: script (dashboard page)

const me = requireRole(["Admin", "Instructor", "Member"]);
if (me) initDashboard();

async function initDashboard() {
    renderNav(me, "home");
    document.getElementById("welcome").textContent = "Welcome, " + me.fullName;

    const links = [{ href: "classes.html", text: "Browse classes" }, { href: "reservations.html", text: "Reservations" }];
    if (me.role === "Admin") {
        links.push({ href: "members.html", text: "Manage members" });
        links.push({ href: "reports.html", text: "Reports" });
    }
    document.getElementById("quickLinks").innerHTML = links
        .map(function (l) { return '<a href="' + l.href + '"><button type="button" class="secondary">' + l.text + "</button></a>"; })
        .join("");

    try {
        if (me.role === "Member") await loadMembership();
        if (me.role === "Admin") await loadOverdueCount();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    }
}

async function loadMembership() {
    document.getElementById("memberCard").hidden = false;
    const m = await apiRequest("/members/me");
    document.getElementById("memberInfo").innerHTML =
        "<p>Status: " + statusBadge(m.membershipStatus) + "</p>" +
        "<p>Plan: <strong>" + escapeHtml(m.membershipPlan || "No plan yet") + "</strong></p>" +
        "<p>Valid until: <strong>" + escapeHtml(formatDate(m.membershipExpiry)) + "</strong></p>";
}

async function loadOverdueCount() {
    document.getElementById("adminCard").hidden = false;
    const overdue = await apiRequest("/reports/overdue-memberships");
    document.getElementById("overdueCount").textContent = overdue.length;
}
