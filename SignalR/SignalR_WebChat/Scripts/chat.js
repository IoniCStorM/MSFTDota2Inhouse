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
function onbtnSignUpClick() {
    if( bSigned == false )
    {
        var NewPlayer = document.createElement("option");
        NewPlayer.text = strUserId;
        sCurrentGamePlayer.options.add( NewPlayer );
        ChatLogAdd( strUserId + " has signed up!\n" );
        
        btnSignUp.innerHTML = "Abandon";
        bSigned = true;
    }
    else
    {
        var i;
        for( i = 0; i < sCurrentGamePlayer.length; i++ )
        {
            if( sCurrentGamePlayer.options[i].text == strUserId )
            {
                sCurrentGamePlayer.remove(i);
                break;
            }
        }
        ChatLogAdd( strUserId + " has abandoned!\n" );
        
        btnSignUp.innerHTML = "Sign Up!";
        bSigned = false;
    }
}

function SendMessage(message) {
    ChatLogAdd(message + "\n");
    tChatInput.value = "";

    // After adding the chat message to the chat area, also
    // send the chat message to the server.
}

function ontChatInputKeyDown(event) {
    if ( event.keyCode == 13 )
    {
        // If the Enter key has been pressed
        if( tChatInput.value != "" )
        {
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