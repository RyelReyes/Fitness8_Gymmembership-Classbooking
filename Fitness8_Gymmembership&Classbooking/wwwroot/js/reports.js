// Owner: Fulgar | Type: script (Reports page - Admin only)

const me = requireRole(["Admin"]);

if (me) {
    renderNav(me, "reports");
    loadOverdue();
    loadAttendance();
}

async function loadOverdue() {
    setTableMessage("overdueBody", 5, "Loading...");
    try {
        const rows = await apiRequest("/reports/overdue-memberships");
        if (rows.length === 0) {
            setTableMessage("overdueBody", 5, "No overdue memberships.");
            return;
        }
        document.getElementById("overdueBody").innerHTML = rows.map(function (r) {
            return "<tr>" +
                "<td>" + escapeHtml(r.fullName) + "</td>" +
                "<td>" + escapeHtml(r.email) + "</td>" +
                "<td>" + escapeHtml(r.membershipPlan || "-") + "</td>" +
                "<td>" + escapeHtml(formatDate(r.membershipExpiry)) + "</td>" +
                "<td>" + statusBadge("Overdue") + " " + r.daysOverdue + " day(s)</td>" +
                "</tr>";
        }).join("");
    } catch (err) {
        setTableMessage("overdueBody", 5, "Could not load this report.");
        showMessage("pageMsg", err.message, "error");
    }
}

async function loadAttendance() {
    setTableMessage("attendanceBody", 7, "Loading...");
    try {
        const rows = await apiRequest("/reports/attendance");
        if (rows.length === 0) {
            setTableMessage("attendanceBody", 7, "No classes yet.");
            return;
        }
        document.getElementById("attendanceBody").innerHTML = rows.map(function (r) {
            return "<tr>" +
                "<td>" + escapeHtml(r.className) + "</td>" +
                "<td>" + escapeHtml(formatDateTime(r.startTime)) + "</td>" +
                "<td>" + r.capacity + "</td>" +
                "<td>" + r.reserved + "</td>" +
                "<td>" + r.confirmed + "</td>" +
                "<td>" + r.attended + "</td>" +
                "<td>" + r.cancelled + "</td>" +
                "</tr>";
        }).join("");
    } catch (err) {
        setTableMessage("attendanceBody", 7, "Could not load this report.");
        showMessage("pageMsg", err.message, "error");
    }
}
