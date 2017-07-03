var taChat = null;
var idUser = null;
var btnSignUp = null;
var tChatInput = null;
var sCurrentGamePlayer = null;
var sPlayer = null;

var strUserId = null;
var intUserMMR = 0;
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
    sCurrentGamePlayer = document.getElementById("sCurrentGamePlayer");
    sPlayer = document.getElementById("sPlayer");
    btnSignUp = document.getElementById("btnSignUp");
    
    intUserMMR = 1500;
    strUserId = idUser.innerHTML + "(" + intUserMMR.toString() + ")";
    
    updatesPlayer();
}

function updatesPlayer() {
    var NewPlayer = document.createElement("option");
    NewPlayer.text = strUserId + " " + playerState.idle;
    sPlayer.options.add( NewPlayer );
}

function UpdatePlayerQueueList(username, Signup) {
    ChatLogAdd("UpdatePlayerQueueList: username:" + username + "\n");
    if(Signup)
    {
        ChatLogAdd("UpdatePlayerQueueList: Signup: true \n" );
        var NewPlayer = document.createElement("option");
        NewPlayer.text = username;
        sCurrentGamePlayer.options.add(NewPlayer);
    }
    else
    {
        ChatLogAdd("UpdatePlayerQueueList: Signup: false \n");
        var i;
        for (i = 0; i < sCurrentGamePlayer.length; i++) {
            if (sCurrentGamePlayer.options[i].text == username) {
                sCurrentGamePlayer.remove(i);
                break;
            }
        }
    }
}

function onbtnSignUpClick() {
    if( bSigned == false )
    {
        $.connection.messageHub.server.signUpForMatch(idUser.innerHTML);
        // Change the button text to Abandon
        btnSignUp.innerHTML = "Abandon";
        bSigned = true;
    }
    else
    {
        $.connection.messageHub.server.abandonSignUpForMatch(idUser.innerHTML);
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

function ChatLogAdd( string )
{
    taChat.value += string;
}