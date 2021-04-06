/// <reference path="oidc-client.js" />

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("welcome_msg").hidden = true;
document.getElementById("login").addEventListener("click", login, false);
//document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
    authority: "https://localhost:6001",
    client_id: "PkceJS",
    redirect_uri: "https://localhost:5001/OpenId/Login/JS",
    response_type: "code",
    scope: "openid profile offline_access MyBackendApi2",
    post_logout_redirect_uri: "https://localhost:5001/OpenId/Login/JS",
    //filterProtocolClaims: true,
    //loadUserInfo: true
};
var mgr = new Oidc.UserManager(config);

//mgr.getUser().then(function (user) {}); // Not work, use the following callback.
mgr.signinRedirectCallback().then(function (user) {
    if (user) {
        document.getElementById("signin_msg").hidden = true;
        document.getElementById("welcome_msg").hidden = false;
        document.getElementById("uid").innerText = user.profile.sub;
        document.getElementById("id_token").innerText = user.id_token;
        document.getElementById("access_token").innerText = user.access_token;
        document.getElementById("refresh_token").innerText = user.refresh_token;
        document.getElementById("expires_at").innerText = moment.unix(user.expires_at).utc();
        log("User logged in", user);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "http://localhost:5001/identity";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    // Delete cookies n local storage
    document.cookie = "idsrv=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;";  
    document.cookie = "idsrv.session=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;";
    location.reload();
    // Signout
    // mgr.signoutRedirect();
}
