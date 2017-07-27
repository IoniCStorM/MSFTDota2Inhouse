<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<?php session_start(); ?>
<?php 
    $GameID = $_POST['GameID'];
    $PlayerID = $_SESSION['username'];
    $GameCreationTime = "7/8/2015 13:45";
    $HostPlayerID = "";
    $LobbyPassword = "ABCD";
    ?>
<head>
    <title>Game <?php echo $GameID?></title>
    <script type="text/javascript" src="Scripts/GamePage.js"></script>
    <style>
table, th, td {
    border: 1px solid black;
}
</style>
</head>
<body onload="onPageLoad()">
    <table align="center">
        <tr><th colspan="2"><strong> <center>Game ID: <span id="spanGameID"> <?php echo $GameID?> </span></center> </strong></th></tr>
        <tr><th colspan="2" id="GameCreationTime"><strong> <center>Game creation time: <time><?php echo $GameCreationTime?></time></center> </strong></th></tr>
        <tr><th colspan="2" id="LobbyPassword"><strong> <center>Password: <span id="spanLobbyPassword"><?php echo $LobbyPassword?> </span></center> </strong></th></tr>
        <tr>
                <tr>
                    <td align="center"><strong>Radiant</strong></td>
                    <td align="center"><strong>Dire</strong></td>
                </tr>
                <tr>
                    <td><strong>Player1</strong></td>
                    <td><strong>Player2</strong></td>
                </tr>
                <tr>
                    <td>Player3</td>
                    <td>Player4</td>
                </tr>
                <tr>
                    <td>Player5</td>
                    <td>Player6</td>
                </tr>
                <tr>
                    <td>Player7</td>
                    <td>Player8</td>
                </tr>
                <tr>
                    <td>Player9</td>
                    <td>Player10</td>
                </tr>
        </tr>
        <tr><th colspan="2"><strong>Lobby Host: <span id="spanLobbyHost"><?php echo $HostPlayerID?></span></strong></th></tr>
        <tr><th colspan="2">
            <button id="btnWin" onclick="onbtnWinClick()">Win</button>
            <button id="btnLoss" onclick="onbtnLossClick()">Loss</button>
            <button id="btnDNF" onclick="onbtnDNFClick()">Game not ended</button>
            </th></tr>
    </table>
</body>
</html>