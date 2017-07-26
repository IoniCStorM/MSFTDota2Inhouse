var taChat = null;
var idUser = null;
var btnSignUp = null;
var tChatInput = null;
var sSignedUpPlayers = null;
var sPlayer = null;
var sGames = null;
var UserMMR = null;

var strUserId = null;
var bSigned = false;

playerState = {
    idle    : "",
    queue   : "Queueing",
    ingame  : "In-Game"
}

function onPageLoad() {
    taChat = document.getElementById("taChat");
    idUser = document.getElementById("idUser");
    tChatInput = document.getElementById("tChatInput");
    sSignedUpPlayers = document.getElementById("sSignedUpPlayers");
    sPlayer = document.getElementById("sPlayer");
    sGames = document.getElementById("sGames");
    btnSignUp = document.getElementById("btnSignUp");
    UserMMR = document.getElementById("UserMMR");
    
    strUserId = idUser.innerText + "(" + UserMMR.innerText + ")";

}

function UpdatePlayerList(UserName, Add) {
    if (Add)
    {
        ChatLogAdd("UpdatePlayerList: Add: true, UserName: " + UserName + "\n");
        var Player = document.createElement("option");
        Player.text = UserName;
        sPlayer.options.add(Player);
    }
    else
    {
        ChatLogAdd("UpdatePlayerList: Add: false, UserName: " + UserName +"\n");
        var i;
        for (i = 0; i < sPlayer.length; i++) {
            if (sPlayer.options[i].text == UserName) {
                sPlayer.remove(i);
                break;
            }
        }
    }
}

function UpdatePlayerQueueList(username, Signup) {
    ChatLogAdd("UpdatePlayerQueueList: username:" + username + "\n");
    if(Signup)
    {
        ChatLogAdd("UpdatePlayerQueueList: Signup: true \n" );
        var NewPlayer = document.createElement("option");
        NewPlayer.text = username;
        sSignedUpPlayers.options.add(NewPlayer);
    }
    else
    {
        ChatLogAdd("UpdatePlayerQueueList: Signup: false \n");
        var i;
        for (i = 0; i < sSignedUpPlayers.length; i++) {
            if (sSignedUpPlayers.options[i].text == username) {
                sSignedUpPlayers.remove(i);
                break;
            }
        }
    }
}

function UpdatePlayerMMR(MMR) {
    UserMMR.innerText = MMR;
}

function onbtnSignUpClick() {
    if( bSigned == false )
    {
        ChatLogAdd("Signing up!\n");
        $.connection.messageHub.server.signUpForMatch(idUser.innerHTML, true);
        // Change the button text to Abandon
        btnSignUp.innerHTML = "Abandon";
        bSigned = true;
    }
    else
    {
        ChatLogAdd("Abandonning!\n");
        $.connection.messageHub.server.signUpForMatch(idUser.innerHTML, false);
        // Change the button text to Sign Up!
        btnSignUp.innerHTML = "Sign Up!";
        bSigned = false;
    }
}

function SendMessage(message) {
    // Clear the input first 
    tChatInput.value = "";

    // ChatLogAdd(message + "\n");

    // After adding the chat message to the chat area, also
    // send the chat message to the server.

    $.connection.messageHub.server.broadCastMessage(strUserId, message);

}

function ontChatInputKeyDown(event) {
    if (event.keyCode == 13) {
        // If the Enter key has been pressed
        if (tChatInput.value != "") {
            SendMessage(tChatInput.value);
        }
    }
}

function onsPlayerdblclick() {
    ChatLogAdd("123 \n");
    
}

function onsGamesdblclick() {
    var form = document.createElement("form");
    form.setAttribute("method", "post");
    form.setAttribute("action", "GamePage.php");
    form.setAttribute("target", "view");

    var hiddenField = document.createElement("input");

    hiddenField.setAttribute("type", "hidden");
    hiddenField.setAttribute("name", "GameID");
    hiddenField.setAttribute("value", sGames.options[sGames.selectedIndex].text);

    form.appendChild(hiddenField);
    document.body.appendChild(form);

    window.open('', 'view');

    form.submit();
}

function ChatLogAdd( string )
{
    taChat.value += string;
}