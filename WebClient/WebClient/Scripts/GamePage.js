var btnWin = null;
var btnLoss = null;
var btnDNF = null;
var LobbyPassword = null;
var UserID = 0;
var GameID = 0;

function onPageLoad() {
    btnWin = document.getElementById("btnWin");
    btnLoss = document.getElementById("btnLoss");
    btnDNF = document.getElementById("btnDNF");
    LobbyPassword = document.getElementById("LobbyPassword");
    GameID = document.getElementById("spanGameID").innerText;
}

function onbtnWinClick() {
    btnWin.innerText = GameID.innerText;
    DisableAllButtons();
}

function onbtnLossClick() {
    DisableAllButtons();
}

function onbtnDNFClick() {
    // DNF stands for Did Not Finish
    DisableAllButtons();
}

function DisableAllButtons() {
    btnWin.disabled = true;
    btnLoss.disabled = true;
    btnDNF.disabled = true;
}