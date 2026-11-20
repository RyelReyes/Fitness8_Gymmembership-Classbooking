// Owner: Reyes | Type: script (navigation bar + footer with logo and Facebook link)

const FACEBOOK_URL = "https://www.facebook.com/fitness8ph";

function renderNav(user, activePage) {
    const links = [{ id: "home", href: "index.html", text: "Dashboard", roles: ["Admin", "Instructor", "Member"] }];
    links.push({ id: "classes", href: "classes.html", text: "Classes", roles: ["Admin", "Instructor", "Member"] });
    links.push({ id: "reservations", href: "reservations.html", text: user.role === "Member" ? "My Reservations" : "Reservations", roles: ["Admin", "Instructor", "Member"] });
    links.push({ id: "members", href: "members.html", text: "Members", roles: ["Admin"] });
    links.push({ id: "reports", href: "reports.html", text: "Reports", roles: ["Admin"] });

    const items = links
        .filter(function (l) { return l.roles.includes(user.role); })
        .map(function (l) {
            return '<a href="' + l.href + '"' + (l.id === activePage ? ' class="active"' : "") + ">" + l.text + "</a>";
        })
        .join("");

    document.getElementById("nav").innerHTML =
        '<a class="brand" href="index.html"><img src="images/logo-fitness8.png" alt="Fitness 8" /></a>' + items +
        '<span class="spacer"></span>' +
        '<span class="who">' + escapeHtml(user.fullName) + " (" + escapeHtml(user.role) + ")</span>" +
        '<button type="button" class="small secondary" id="logoutBtn">Log out</button>';

    document.getElementById("logoutBtn").addEventListener("click", logout);
    renderFooter();
}

function renderFooter() {
    const footer = document.createElement("footer");
    footer.className = "footer";
    footer.innerHTML =
        '<img src="images/logo-quezon-ave.png" alt="Fitness 8 Quezon Avenue" />' +
        "<div>Fitness 8 - Quezon Avenue, Quezon City<br /><span class=\"muted\">Where your fitness journey begins</span></div>" +
        '<a class="btn-link" href="' + FACEBOOK_URL + '" target="_blank" rel="noopener">Follow us on Facebook</a>';
    document.body.appendChild(footer);
}
