<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<?php session_start(); ?>
<head>
    <title>Welcome to Microsoft Dota2 InHouse League</title>
    <script type="text/javascript" src="Scripts/main.js"></script>
</head>
<body onload="onPageLoad()">
    <script src="Scripts/jquery-1.6.4.min.js"></script>
    <script src="Scripts/jquery.signalR-2.2.2.min.js"></script>
    <script src="SignalR/hubs"></script>

    <form action="index.php" method="GET">
        <input type="submit" value="Log Out">
    </form>
    The Server Time is:<span id="serverTime"></span>
    
    <?php
    // Ensure the user has logged in
    if( $_SESSION['username'] == "" )
    {
        $_SESSION['errormsg'] = "Unknown user.<br>";
        // Redirects back to the login page.
        header('Location: index.php');
        exit;
    }
    ?>
    
    <script>
        idUser = document.getElementById("idUser");

        $.connection.hub.start().done(function () {
            $.connection.messageHub.server.broadCastServerTime();
            $.connection.messageHub.server.connectUser(idUser.innerHTML);
            $.connection.messageHub.server.populateCurrentPlayerList();
            $.connection.messageHub.server.populateCurrentPlayerQueueList();
        });

        $.connection.messageHub.client.MessageReceiver = function (message) {
            $("#serverTime").text(message);
        }

        $.connection.messageHub.client.ChatMessageReceiver = function (message) {
            ChatLogAdd(message);
        }

        $.connection.messageHub.client.UpdatePlayerQueueListReceiver = function (username, Signup) {
            UpdatePlayerQueueList(username, Signup);
        }

        $.connection.messageHub.client.UpdateOnlinePlayerListReceiver = function (username, Add) {
            UpdatePlayerList(username, Add);
        }

        $.connection.messageHub.client.UpdatePlayerMMRReceiver = function (MMR) {
            UpdatePlayerMMR(MMR);
        }
        
        $.connection.messageHub.client.UpdateGameListReceiver = function (GameID, Add) {
            UpdateGameList(GameID, Add)
        }

    </script>

    <table style="width: 100%;">
        <tr>
            <td style="width:20%">
                <table>
                    <tr style="height: 20%; width:20%">
                        <td style="width: 20%; vertical-align:top">
                            <!--Current signed up player list-->
                            Signed up Players: <button id="btnSignUp" onclick="onbtnSignUpClick()">Sign Up!</button>
                            <select id="sSignedUpPlayers" size="10" style="width:100%">
                            </select>
                        </td>
                    </tr>
                    <tr style="height: 60%; width:20%;">
                        <td style="width: 20%; vertical-align:top">
                            <!--Player list-->
                            Players On-line:
                            <select id="sPlayer" size="12" ondblclick="onsPlayerdblclick()" style="width:100%"/>
                        </td>
                    </tr>
                    <tr style="height: 60%; width:20%;">
                        <td style="width: 20%; vertical-align:top">
                            <!--Current game list-->
                            Ongoing Games:
                            <select id="sGames" size="12" ondblclick="onsGamesdblclick()" style="width:100%">
                            <option>123</option>
                            </select>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width:80%; vertical-align:top">
                <table style="width:100%">
                    <tr>
                        <td>
                            <strong id="idUser"><?php echo($_SESSION['username']);?></strong><strong> - </strong><strong id="UserMMR">0</strong></br>
                            Chat:
                            <textarea id="taChat" rows="19" col="auto" readonly style="background-color:white; width:100%; height:inherit"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width:100%">
                                <tr>
                                    <td style="width:90%">
                                        <input id="tChatInput" type="text" onkeydown="ontChatInputKeyDown(event)" style="width:100%" />
                                    </td>
                                    <td>
                                        <input id="btnSend" type="button" style="width:100%" Value="Send" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</body>
</html>
