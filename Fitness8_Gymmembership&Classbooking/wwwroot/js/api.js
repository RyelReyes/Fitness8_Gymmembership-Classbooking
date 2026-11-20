// Owner: Reyes | Type: script (shared API helper: base URL, token, authFetch, guards)
// ONE place for the API URL, token handling and 401/403 handling (rubric: no copy-pasted token code).

const API_BASE = "";                       // same origin: the API serves this front end from wwwroot
const API_URL = API_BASE + "/api/v1";
const TOKEN_KEY = "gym_token";
const USER_KEY = "gym_user";

function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

function getUser() {
    try {
        return JSON.parse(localStorage.getItem(USER_KEY));
    } catch (e) {
        return null;
    }
}

function saveSession(auth) {
    localStorage.setItem(TOKEN_KEY, auth.token);
    localStorage.setItem(USER_KEY, JSON.stringify({ username: auth.username, fullName: auth.fullName, role: auth.role }));
}

function clearSession() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
}

function logout() {
    clearSession();
    window.location.href = "login.html";
}

// Guard a page: not logged in -> login page; wrong role -> dashboard. Returns the user or null.
function requireRole(allowedRoles) {
    const user = getUser();
    if (!getToken() || !user) {
        window.location.href = "login.html";
        return null;
    }
    if (allowedRoles && !allowedRoles.includes(user.role)) {
        window.location.href = "index.html";
        return null;
    }
    return user;
}

// fetch() + automatic "Authorization: Bearer <token>" + 401 handling.
async function authFetch(path, options = {}) {
    const headers = Object.assign({}, options.headers);
    const token = getToken();
    if (token) headers["Authorization"] = "Bearer " + token;
    if (options.body && !headers["Content-Type"]) headers["Content-Type"] = "application/json";

    let res;
    try {
        res = await fetch(API_URL + path, Object.assign({}, options, { headers: headers }));
    } catch (e) {
        throw new Error("Cannot reach the server. Is the API running?");
    }

    if (res.status === 401 && token) {          // token missing/expired -> back to login
        clearSession();
        window.location.href = "login.html";
        throw new Error("Your session expired. Please log in again.");
    }
    return res;
}

// Turn any error response (our { error }, validation { errors }, or empty 403/404) into one readable message.
async function readError(res) {
    let data = null;
    try {
        data = await res.json();
    } catch (e) {
        data = null;
    }
    if (data && data.error) return data.error;
    if (data && data.errors) return Object.values(data.errors).flat().join(" ");
    if (res.status === 403) return "You are not allowed to do that.";
    if (res.status === 404) return "That record was not found.";
    return "Something went wrong (HTTP " + res.status + ").";
}

// authFetch + JSON parsing + throws Error(message) on any non-2xx.
async function apiRequest(path, options = {}) {
    const res = await authFetch(path, options);
    if (!res.ok) throw new Error(await readError(res));
    if (res.status === 204) return null;
    return await res.json();
}
