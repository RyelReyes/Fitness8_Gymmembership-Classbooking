// Owner: Salvador | Type: script (Classes page - everyone views; Admin manages; Member reserves)

const me = requireRole(["Admin", "Instructor", "Member"]);
const isAdmin = me !== null && me.role === "Admin";
const isMember = me !== null && me.role === "Member";
const isStaff = isAdmin || (me !== null && me.role === "Instructor");

let classes = [];
let editingId = null;

if (me) initPage();

async function initPage() {
    renderNav(me, "classes");
    document.getElementById("adminPanel").hidden = !isAdmin;
    document.getElementById("classesBody").addEventListener("click", onTableClick);
    document.getElementById("closeRoster").addEventListener("click", function () {
        document.getElementById("rosterCard").hidden = true;
    });

    if (isAdmin) {
        document.getElementById("classForm").addEventListener("submit", saveClass);
        document.getElementById("cancelBtn").addEventListener("click", resetForm);
        await loadInstructors();
    }
    await loadClasses();
}

async function loadInstructors() {
    try {
        const instructors = await apiRequest("/classes/instructors");
        document.getElementById("instructor").innerHTML = instructors
            .map(function (i) { return '<option value="' + i.id + '">' + escapeHtml(i.name) + "</option>"; })
            .join("");
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    }
}

async function loadClasses() {
    setTableMessage("classesBody", 6, "Loading classes...");
    try {
        // Members only need upcoming classes; staff see everything that is not cancelled.
        classes = await apiRequest("/classes");
        if (isMember) {
            const now = new Date();
            classes = classes.filter(function (c) {
                return c.status === "Open" && new Date(c.startTime) > now;
            });
        }
        renderClasses();
    } catch (err) {
        setTableMessage("classesBody", 6, "Could not load classes.");
        showMessage("pageMsg", err.message, "error");
    }
}

function renderClasses() {
    if (classes.length === 0) {
        setTableMessage("classesBody", 6, "No classes scheduled.");
        return;
    }

    const now = new Date();
    document.getElementById("classesBody").innerHTML = classes.map(function (c) {
        const started = new Date(c.startTime) <= now;
        const full = Number(c.available) <= 0;

        let actions = "";
        if (isMember) {
            actions = full || started
                ? statusBadge(full ? "Full" : "Started")
                : '<button class="small" data-action="reserve" data-id="' + c.id + '">Reserve</button>';
        }
        if (isStaff) {
            actions += '<button class="small secondary" data-action="roster" data-id="' + c.id + '">Roster</button>';
        }
        if (isAdmin) {
            actions += '<button class="small secondary" data-action="edit" data-id="' + c.id + '">Edit</button>' +
                '<button class="small danger" data-action="cancel" data-id="' + c.id + '">Cancel class</button>';
        }

        return "<tr>" +
            "<td>" + escapeHtml(c.name) + "<div class='hint'>" + escapeHtml(c.description || "") + "</div></td>" +
            "<td>" + escapeHtml(c.instructorName) + "</td>" +
            "<td>" + escapeHtml(formatDateTime(c.startTime)) + "</td>" +
            "<td>" + c.durationMinutes + " min</td>" +
            "<td>" + c.reserved + " / " + c.capacity + (full ? " " + statusBadge("Full") : "") + "</td>" +
            "<td>" + actions + "</td></tr>";
    }).join("");
}

async function onTableClick(e) {
    const button = e.target.closest("button[data-action]");
    if (!button) return;

    const id = Number(button.dataset.id);
    const action = button.dataset.action;

    if (action === "reserve") await reserveClass(button, id);
    if (action === "roster") await showRoster(id);
    if (action === "edit") startEdit(id);
    if (action === "cancel") await cancelClass(button, id);
}

// ---------- Member: reserve ----------
async function reserveClass(button, id) {
    showMessage("pageMsg", "", "info");
    setBusy(button, true, "Booking...");
    try {
        const r = await apiRequest("/reservations", { method: "POST", body: JSON.stringify({ classId: id }) });
        showMessage("pageMsg", 'Booked "' + r.className + '". Status: ' + r.status + ".", "success");
        await loadClasses();
    } catch (err) {
        // e.g. "Class is already full", "Your membership expired...", "Schedule conflict..."
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
}

// ---------- Staff: roster ----------
async function showRoster(id) {
    try {
        const rows = await apiRequest("/classes/" + id + "/roster");
        const c = classes.find(function (x) { return x.id === id; });
        document.getElementById("rosterTitle").textContent = "Roster: " + (c ? c.name : "class");
        document.getElementById("rosterBody").innerHTML = rows.length === 0
            ? '<tr><td class="center" colspan="3">No reservations yet.</td></tr>'
            : rows.map(function (r) {
                return "<tr><td>" + escapeHtml(r.memberName) + "</td><td>" + statusBadge(r.status) + "</td><td>" + escapeHtml(formatDateTime(r.createdAt)) + "</td></tr>";
            }).join("");
        document.getElementById("rosterCard").hidden = false;
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    }
}

// ---------- Admin: create / edit / cancel ----------
function startEdit(id) {
    const c = classes.find(function (x) { return x.id === id; });
    if (!c) return;

    editingId = id;
    document.getElementById("formTitle").textContent = "Edit class";
    document.getElementById("cancelBtn").hidden = false;
    document.getElementById("name").value = c.name;
    document.getElementById("instructor").value = c.instructorId;
    document.getElementById("startTime").value = c.startTime.substring(0, 16);
    document.getElementById("duration").value = c.durationMinutes;
    document.getElementById("capacity").value = c.capacity;
    document.getElementById("description").value = c.description || "";
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function resetForm() {
    editingId = null;
    document.getElementById("classForm").reset();
    document.getElementById("duration").value = 60;
    document.getElementById("capacity").value = 10;
    document.getElementById("formTitle").textContent = "Add class";
    document.getElementById("cancelBtn").hidden = true;
}

function validateForm() {
    if (!document.getElementById("name").value.trim()) return "Class name is required.";
    if (!document.getElementById("instructor").value) return "Please choose an instructor.";
    const start = document.getElementById("startTime").value;
    if (!start) return "Please choose a start date and time.";
    if (new Date(start) <= new Date()) return "Start time must be in the future.";
    const duration = Number(document.getElementById("duration").value);
    if (!(duration >= 15 && duration <= 240)) return "Duration must be between 15 and 240 minutes.";
    const capacity = Number(document.getElementById("capacity").value);
    if (!(capacity >= 1 && capacity <= 200)) return "Capacity must be between 1 and 200.";
    return "";
}

async function saveClass(e) {
    e.preventDefault();
    showMessage("pageMsg", "", "info");

    const problem = validateForm();
    if (problem) {
        showMessage("pageMsg", problem, "error");
        return;
    }

    const button = document.getElementById("saveBtn");
    setBusy(button, true, "Saving...");

    const body = {
        name: document.getElementById("name").value.trim(),
        description: document.getElementById("description").value.trim() || null,
        instructorId: Number(document.getElementById("instructor").value),
        startTime: document.getElementById("startTime").value,
        durationMinutes: Number(document.getElementById("duration").value),
        capacity: Number(document.getElementById("capacity").value)
    };

    try {
        if (editingId === null) {
            await apiRequest("/classes", { method: "POST", body: JSON.stringify(body) });
            showMessage("pageMsg", "Class added.", "success");
        } else {
            await apiRequest("/classes/" + editingId, { method: "PUT", body: JSON.stringify(body) });
            showMessage("pageMsg", "Class updated.", "success");
        }
        resetForm();
        await loadClasses();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    } finally {
        setBusy(button, false);
    }
}

async function cancelClass(button, id) {
    if (!confirm("Cancel this class? Open reservations will be cancelled too.")) return;
    setBusy(button, true, "...");
    try {
        await apiRequest("/classes/" + id, { method: "DELETE" });
        showMessage("pageMsg", "Class cancelled.", "success");
        await loadClasses();
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
}
