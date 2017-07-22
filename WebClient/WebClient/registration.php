<html>
    <?php session_start(); ?>
    <head>
        <title>
            Microsoft Dota2 InHouse Registration
        </title>
    </head>
    <body>
        <h2 align="center">
            Microsoft Dota2 InHouse Registration
        </h2>
        
        <?php 
        if( !empty( $_POST['username'] )
         && !empty( $_POST['password'] )
         && !empty( $_POST['steamprofile'] ) )
        {
            $username       = $_POST['username'];
            $password       = $_POST['password'];
            $steamprofile   = $_POST['steamprofile'];
            $pos1           = convertPos( $_POST['position1'] );
            $pos2           = convertPos( $_POST['position2'] );
            
            echo "Saving data to database...<br>";
			$mysqli = new mysqli($_SESSION['db_server'],$_SESSION['db_username'],$_SESSION['db_password'],'userinfo');
			if ($mysqli->connect_errno) {
				echo "Failed to connect to MySQL(" . $mysqli->connect_errno . ") " . $mysqli->connect_error;
			}

			if (!($stmt = $mysqli->prepare("SELECT * FROM `basicinfo` WHERE `username` LIKE ?"))) {
				echo "Prepare failed: (" . $mysqli->errno . ") " . $mysqli->error;
			}

			if(!$stmt->bind_param('s', $username)) {
				echo "Binding parameters failed: (" . $stmt->errno . ") " . $stmt->error;
			}

			if(!$stmt->execute()){
				echo "Execute failed: (" . $stmt->errno . ") " . $stmt->error;
			}

			$fetched_result = $stmt->get_result()->fetch_assoc();
            
            if( $fetched_result == FALSE )
            {
                echo "This user does not exist. <br>";
            }
            else
            {
                echo "This user name already exists, please use another user name. <br>";
                $RegistrationPhase = 1;
                goto Form;
            }
            
            $password_hashed = password_hash($password, PASSWORD_DEFAULT);
			if (!($stmt = $mysqli->prepare("INSERT INTO `userinfo`.`basicinfo` (`username`, `password`, `steamprofile`, `1stPos`, `2ndPos`) VALUES ( ?, ?, ?, ?, ?);"))) {
				echo "Prepare failed: (" . $mysqli->errno . ") " . $mysqli->error;
			}

			if(!$stmt->bind_param('sssss', $username, $password_hashed, $steamprofile, $pos1, $pos2)) {
				echo "Binding parameters failed: (" . $stmt->errno . ") " . $stmt->error;
			}

			if($stmt->execute()){
				echo "Registration succeeded, please return to the login page and login.<br>";
                $_SESSION['username']       = $username;
                printf("
                <form action='index.php' method='POST'>
                    <input type='submit' value='Return'/>
                </form>
                ");
			}
			else
			{
				echo "Execute failed: (" . $stmt->errno . ") " . $stmt->error . "<br>";
				echo "Registration failed, please contact admin (alias:Dota2InHouseAdmin) for help. <br>";
			}
            
            $RegistrationPhase = 2;
        }
        else
        {
            $RegistrationPhase = 1;
        }
        ?>
        
        <?php
Form:
        if( $RegistrationPhase == 1 )
        {
            printForm();
        }
        ?>
    </body>

</html>

<?php
    function printForm() {
        echo("
        <form action='" . htmlspecialchars($_SERVER['PHP_SELF']) . "' method='POST'>
            <table align='center'>
                <tr>
                    <td>
                        User name:
                    </td>
                    <td>
                        <input name='username' type='text'/>
                    </td>
                </tr>
                <tr>
                    <td>
                        Password:
                    </td>
                    <td>
                        <input name='password' type='password'/>
                    </td>
                </tr>
                <tr>
                    <td>
                        Steam profile URL:
                    </td>
                    <td>
                        <input name='steamprofile' type='text'/>
                    </td>
                </tr>
                <tr>
                    <td>
                        1st preferred position:
                    </td>
                    <td>
                        <div TITLE='Carry means position 1, Ganker means position 2/3 and Support means position 4/5'>
                        <select name='position1'>
                            <option value='All'>All-around</option>
                            <option value='Carry'>Carry</option>
                            <option value='Ganker'>Ganker</option>
                            <option value='Support'>Support</option>
                        </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        2nd preferred position:
                    </td>
                    <td>
                        <div TITLE='Carry means position 1, Ganker means position 2/3 and Support means position 4/5'>
                        <select name='position2'>
                            <option value='All'>All-around</option>
                            <option value='Carry'>Carry</option>
                            <option value='Ganker'>Ganker</option>
                            <option value='Support'>Support</option>
                        </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align='right'>
                        <input type='submit' value='Submit'/>
                    </td>
                </form>
                <form action='index.php' method='POST'>
                    <td align='left'>
                        <input type='submit' value='Return'/>
                    </td>
                </form>
                </tr>
            </table>
        ");
    }
    
    function convertPos( $Pos )
    {
        switch( $Pos )
        {
            case 'All-around':
                return 'A';
            break;
            case 'Carry':
                return 'C';
            break;
            case 'Ganker':
                return 'G';
            break;
            case 'Support':
                return 'S';
            break;
            default:
                return 'A';
            break;
        }
    }
?>