// Owner: Fulgar | Type: script (Members page - Admin only)

const me = requireRole(["Admin"]);
let members = [];
let editingId = null;

if (me) {
    renderNav(me, "members");
    document.getElementById("memberForm").addEventListener("submit", saveMember);
    document.getElementById("cancelBtn").addEventListener("click", resetForm);
    document.getElementById("closeDetail").addEventListener("click", function () {
        document.getElementById("detailCard").hidden = true;
    });
    document.getElementById("membersBody").addEventListener("click", onTableClick);
    loadMembers();
}

async function loadMembers() {
    setTableMessage("membersBody", 6, "Loading members...");
    try {
        members = await apiRequest("/members");
        renderMembers();
    } catch (err) {
        setTableMessage("membersBody", 6, "Could not load members.");
        showMessage("pageMsg", err.message, "error");
    }
}

function renderMembers() {
    if (members.length === 0) {
        setTableMessage("membersBody", 6, "No members yet. Add the first one above.");
        return;
    }

    document.getElementById("membersBody").innerHTML = members.map(function (m) {
        const renewButtons = m.isActive
            ? '<button class="small secondary" data-action="renew" data-id="' + m.id + '" data-plan="Monthly" data-days="30">+30d</button>' +
            '<button class="small secondary" data-action="renew" data-id="' + m.id + '" data-plan="Quarterly" data-days="90">+90d</button>' +
            '<button class="small secondary" data-action="renew" data-id="' + m.id + '" data-plan="Annual" data-days="365">+1yr</button>' +
            '<button class="small danger" data-action="deactivate" data-id="' + m.id + '">Deactivate</button>'
            : "";

        return "<tr>" +
            "<td>" + escapeHtml(m.fullName) + "<div class='hint'>" + escapeHtml(m.email) + "</div></td>" +
            "<td>" + escapeHtml(m.username) + "</td>" +
            "<td>" + escapeHtml(m.membershipPlan || "-") + "</td>" +
            "<td>" + escapeHtml(formatDate(m.membershipExpiry)) + "</td>" +
            "<td>" + statusBadge(m.membershipStatus) + "</td>" +
            "<td>" +
            '<button class="small secondary" data-action="view" data-id="' + m.id + '">View</button>' +
            '<button class="small secondary" data-action="edit" data-id="' + m.id + '">Edit</button>' +
            renewButtons +
            "</td></tr>";
    }).join("");
}

async function onTableClick(e) {
    const button = e.target.closest("button[data-action]");
    if (!button) return;

    const id = Number(button.dataset.id);
    const action = button.dataset.action;

    if (action === "view") await viewMember(id);
    if (action === "edit") startEdit(id);
    if (action === "renew") await renewMember(button, id, button.dataset.plan, Number(button.dataset.days));
    if (action === "deactivate") await deactivateMember(button, id);
}

async function viewMember(id) {
    try {
        const m = await apiRequest("/members/" + id);   // GET one member (detail view)
        document.getElementById("detailBody").innerHTML =
            "<p><strong>" + escapeHtml(m.fullName) + "</strong> (" + escapeHtml(m.username) + ")</p>" +
            "<p>Email: " + escapeHtml(m.email) + "</p>" +
            "<p>Phone: " + escapeHtml(m.phone || "-") + "</p>" +
            "<p>Plan: " + escapeHtml(m.membershipPlan || "-") + " | Valid until: " + escapeHtml(formatDate(m.membershipExpiry)) + "</p>" +
            "<p>Status: " + statusBadge(m.membershipStatus) + "</p>";
        document.getElementById("detailCard").hidden = false;
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    }
}

function startEdit(id) {
    const m = members.find(function (x) { return x.id === id; });
    if (!m) return;

    editingId = id;
    document.getElementById("formTitle").textContent = "Edit member";
    document.getElementById("usernameField").hidden = true;      // username/password are set only on create
    document.getElementById("passwordField").hidden = true;
    document.getElementById("activeField").hidden = false;
    document.getElementById("cancelBtn").hidden = false;

    document.getElementById("fullName").value = m.fullName;
    document.getElementById("email").value = m.email;
    document.getElementById("phone").value = m.phone || "";
    document.getElementById("plan").value = m.membershipPlan || "";
    document.getElementById("expiry").value = m.membershipExpiry ? m.membershipExpiry.substring(0, 10) : "";
    document.getElementById("isActive").checked = m.isActive;
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function resetForm() {
    editingId = null;
    document.getElementById("memberForm").reset();
    document.getElementById("formTitle").textContent = "Add member";
    document.getElementById("usernameField").hidden = false;
    document.getElementById("passwordField").hidden = false;
    document.getElementById("activeField").hidden = true;
    document.getElementById("cancelBtn").hidden = true;
}

// Client-side validation: friendly messages BEFORE we call the API (the API validates again).
function validateForm() {
    const fullName = document.getElementById("fullName").value.trim();
    const email = document.getElementById("email").value.trim();
    const plan = document.getElementById("plan").value;
    const expiry = document.getElementById("expiry").value;

    if (!fullName) return "Full name is required.";
    if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) return "Please enter a valid email address.";
    if (expiry && !plan) return "Choose a membership plan when you set a valid-until date.";

    if (editingId === null) {
        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value;
        if (username.length < 3) return "Username must be at least 3 characters.";
        if (password.length < 6) return "Password must be at least 6 characters.";
    }
    return "";
}

async function saveMember(e) {
    e.preventDefault();
    showMessage("pageMsg", "", "info");

    const problem = validateForm();
    if (problem) {
        showMessage("pageMsg", problem, "error");
        return;
    }

    const button = document.getElementById("saveBtn");
    setBusy(button, true, "Saving...");

    const expiry = document.getElementById("expiry").value;
    const body = {
        fullName: document.getElementById("fullName").value.trim(),
        email: document.getElementById("email").value.trim(),
        phone: document.getElementById("phone").value.trim() || null,
        membershipPlan: document.getElementById("plan").value || null,
        membershipExpiry: expiry || null
    };

    try {
        if (editingId === null) {
            body.username = document.getElementById("username").value.trim();
            body.password = document.getElementById("password").value;
            await apiRequest("/members", { method: "POST", body: JSON.stringify(body) });
            showMessage("pageMsg", "Member added.", "success");
        } else {
            body.isActive = document.getElementById("isActive").checked;
            await apiRequest("/members/" + editingId, { method: "PUT", body: JSON.stringify(body) });
            showMessage("pageMsg", "Member updated.", "success");
        }
        resetForm();
        await loadMembers();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    } finally {
        setBusy(button, false);
    }
}

async function renewMember(button, id, plan, days) {
    setBusy(button, true, "...");
    try {
        const updated = await apiRequest("/members/" + id + "/renew", { method: "PATCH", body: JSON.stringify({ plan: plan, days: days }) });
        showMessage("pageMsg", updated.fullName + " is now valid until " + formatDate(updated.membershipExpiry) + ".", "success");
        await loadMembers();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
}

async function deactivateMember(button, id) {
    if (!confirm("Deactivate this member? Their open reservations will be cancelled.")) return;
    setBusy(button, true, "...");
    try {
        await apiRequest("/members/" + id, { method: "DELETE" });
        showMessage("pageMsg", "Member deactivated.", "success");
        await loadMembers();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
}
