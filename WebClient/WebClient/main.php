<!DOCTYPE html>
<html>
<?php session_start(); ?>
<?php $_SESSION['ranking'] = 1500; ?>
<head>
    <title>Welcome to Microsoft Dota2 InHouse League</title>
    <script type="text/javascript" src="main.js"></script>
</head>
<body onload="onPageLoad()">
    <table style="width: 100%;" >
        <tr>
            <td style="width:20%">
                    <table>
                        <tr style="height: 20%; width:20%">
                            <td style="width: 20%; vertical-align:top">
                                    <!--Current game list-->
									Signed up Players: <button id="btnSignUp" onclick="onbtnSignUpClick()">Sign Up!</button>
                                    <select id="sCurrentGamePlayer" size="10" style="width:100%">
                                        <option>Player1</option>
                                        <option>Player2</option>
                                    </select>
                            </td>
                        </tr>
                        <tr style="height: 80%; width:20%;">
                            <td style="width: 20%; vertical-align:top">
                                    <!--Player list-->
									Players On-line:
                                    <select id="sPlayer" size="12" ondblclick="onsPlayerdblclick()" style="width:100%">
                                        <option>Player1</option>
                                        <option>Player2</option>
                                        <option>Player3</option>
                                    </select>
                            </td>
                        </tr>
                    </table>
            </td>
			<td style="width:80%; vertical-align:top">
				<table style="width:100%">
					<tr>
						<td>
                            <strong id="idUser"><?php echo($_SESSION['username']);?></strong><strong><?php echo( " - " . $_SESSION['ranking']); ?></strong></br>
                            Chat:
							<textarea id="taChat" rows="19" col="auto" readonly style="background-color:white; width:100%; height:inherit"></textarea>
						</td>
					</tr>
					<tr>
						<td>
                            <table style="width:100%">
                                <tr>
                                    <td style="width:90%">
                                        <input id="tChatInput" type="text" onkeydown="ontChatInputKeyDown(event)" style="width:100%"/>
                                    </td>
                                    <td>
                                        <input id="btnSend" type="button" style="width:100%" Value="Send"/>
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