// Owner: Salvador | Type: script (Reservations page - status workflow buttons)

const me = requireRole(["Admin", "Instructor", "Member"]);
let reservations = [];

// The server tells us which next statuses THIS user may apply (allowedStatuses);
// we only turn them into friendly button labels. Invalid moves never appear.
const ACTION_LABELS = { Confirmed: "Confirm", Attended: "Mark attended", Cancelled: "Cancel" };

if (me) {
    renderNav(me, "reservations");
    document.getElementById("pageTitle").textContent = me.role === "Member" ? "My reservations" : "Reservations";
    document.getElementById("statusFilter").addEventListener("change", renderReservations);
    document.getElementById("reservationsBody").addEventListener("click", onTableClick);
    document.getElementById("closeDetail").addEventListener("click", function () {
        document.getElementById("detailCard").hidden = true;
    });
    loadReservations();
}

async function loadReservations() {
    setTableMessage("reservationsBody", 5, "Loading reservations...");
    try {
        reservations = await apiRequest("/reservations");
        renderReservations();
    } catch (err) {
        setTableMessage("reservationsBody", 5, "Could not load reservations.");
        showMessage("pageMsg", err.message, "error");
    }
}

function renderReservations() {
    const filter = document.getElementById("statusFilter").value;
    const rows = reservations.filter(function (r) { return !filter || r.status === filter; });

    if (rows.length === 0) {
        setTableMessage("reservationsBody", 5, me.role === "Member" ? "You have no reservations yet. Book a class first." : "No reservations found.");
        return;
    }

    document.getElementById("reservationsBody").innerHTML = rows.map(function (r) {
        const buttons = r.allowedStatuses.map(function (next) {
            const style = next === "Cancelled" ? "small danger" : "small";
            return '<button class="' + style + '" data-action="status" data-id="' + r.id + '" data-status="' + next + '">' + ACTION_LABELS[next] + "</button>";
        }).join("");

        return "<tr>" +
            "<td>" + escapeHtml(r.memberName) + "</td>" +
            "<td>" + escapeHtml(r.className) + "</td>" +
            "<td>" + escapeHtml(formatDateTime(r.classStartTime)) + "</td>" +
            "<td>" + statusBadge(r.status) + "</td>" +
            "<td>" + '<button class="small secondary" data-action="view" data-id="' + r.id + '">Details</button>' + buttons + "</td>" +
            "</tr>";
    }).join("");
}

async function onTableClick(e) {
    const button = e.target.closest("button[data-action]");
    if (!button) return;

    const id = Number(button.dataset.id);
    if (button.dataset.action === "view") await viewReservation(id);
    if (button.dataset.action === "status") await changeStatus(button, id, button.dataset.status);
}

async function viewReservation(id) {
    try {
        const r = await apiRequest("/reservations/" + id);    // GET one reservation (detail view)
        document.getElementById("detailBody").innerHTML =
            "<p><strong>" + escapeHtml(r.className) + "</strong> - " + escapeHtml(formatDateTime(r.classStartTime)) + "</p>" +
            "<p>Member: " + escapeHtml(r.memberName) + "</p>" +
            "<p>Status: " + statusBadge(r.status) + "</p>" +
            "<p>Booked on: " + escapeHtml(formatDateTime(r.createdAt)) + "</p>";
        document.getElementById("detailCard").hidden = false;
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    }
}

async function changeStatus(button, id, status) {
    if (status === "Cancelled" && !confirm("Cancel this reservation?")) return;

    showMessage("pageMsg", "", "info");
    setBusy(button, true, "...");
    try {
        const updated = await apiRequest("/reservations/" + id + "/status", { method: "PATCH", body: JSON.stringify({ status: status }) });
        showMessage("pageMsg", 'Reservation for "' + updated.className + '" is now ' + updated.status + ".", "success");
        await loadReservations();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
}
