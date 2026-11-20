// Owner: Reyes | Type: script (login + register page)

// The login page is always the starting point: drop any old session so the user must log in again.
clearSession();

renderFooter();

document.getElementById("loginForm").addEventListener("submit", async function (e) {
    e.preventDefault();
    const button = document.getElementById("loginBtn");
    showMessage("pageMsg", "", "info");
    setBusy(button, true, "Logging in...");

    try {
        const auth = await apiRequest("/auth/login", {
            method: "POST",
            body: JSON.stringify({
                username: document.getElementById("loginUser").value.trim(),
                password: document.getElementById("loginPass").value
            })
        });
        saveSession(auth);
        window.location.href = "index.html";
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
        setBusy(button, false);
    }
});

document.getElementById("registerForm").addEventListener("submit", async function (e) {
    e.preventDefault();
    const button = document.getElementById("registerBtn");
    showMessage("pageMsg", "", "info");
    setBusy(button, true, "Creating account...");

    const username = document.getElementById("regUser").value.trim();

    try {
        await apiRequest("/auth/register", {
            method: "POST",
            body: JSON.stringify({
                username: username,
                password: document.getElementById("regPass").value,
                fullName: document.getElementById("regName").value.trim(),
                email: document.getElementById("regEmail").value.trim(),
                phone: document.getElementById("regPhone").value.trim() || null,
                membershipPlan: document.getElementById("regPlan").value
            })
        });
        document.getElementById("registerForm").reset();          // clear the form on success
        document.getElementById("loginUser").value = username;
        showMessage("pageMsg", "Account created. You can log in now.", "success");
    } catch (err) {
        showMessage("pageMsg", err.message, "error");
    } finally {
        setBusy(button, false);
    }
});
