setTimeout(SetHomeLink, 1000);

function SetHomeLink() {
    let homeAnchor = document.querySelector(".topbar-wrapper a");
    homeAnchor.href = "/Home/Index";
}