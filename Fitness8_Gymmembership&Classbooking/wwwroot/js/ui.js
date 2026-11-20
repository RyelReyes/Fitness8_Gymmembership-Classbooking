// Owner: Reyes | Type: script (shared UI helpers)

function escapeHtml(value) {
    return String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;");
}

// Shows a message box (type: "success" | "error" | "info"); empty text hides it.
function showMessage(elementId, text, type) {
    const el = document.getElementById(elementId);
    if (!el) return;
    el.textContent = text || "";
    el.className = "msg " + (type || "info");
    el.hidden = !text;
}

function formatDate(iso) {
    if (!iso) return "-";
    return new Date(iso).toLocaleDateString("en-PH", { year: "numeric", month: "short", day: "numeric" });
}

function formatDateTime(iso) {
    if (!iso) return "-";
    return new Date(iso).toLocaleString("en-PH", { dateStyle: "medium", timeStyle: "short" });
}

function statusBadge(text) {
    return '<span class="badge badge-' + escapeHtml(String(text).toLowerCase()) + '">' + escapeHtml(text) + "</span>";
}

// One row that spans the table: used for "Loading..." and "No records" states.
function setTableMessage(tbodyId, colspan, text) {
    document.getElementById(tbodyId).innerHTML =
        '<tr><td class="center" colspan="' + colspan + '">' + escapeHtml(text) + "</td></tr>";
}

// Disable a button while a request is running (prevents double submit).
function setBusy(button, busy, busyText) {
    if (!button) return;
    if (busy) {
        button.dataset.label = button.textContent;
        button.textContent = busyText || "Please wait...";
    } else if (button.dataset.label) {
        button.textContent = button.dataset.label;
    }
    button.disabled = busy;
}
