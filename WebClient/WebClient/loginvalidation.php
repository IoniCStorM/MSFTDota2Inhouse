<html>
    <?php session_start(); ?>
    
    <?php
    // Initializing GLOBAL SESSION variables
    $_SESSION['db_server']      = 'localhost';
    $_SESSION['db_username']    = 'WebClient';
    $_SESSION['db_password']    = 'MSFTDota2InHouse';
    $_SESSION['username']       = $_POST['username'];
    $_SESSION['errormsg']       = '';
    
    $username = $_POST['username'];

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

	if ($fetched_result == FALSE)
	{
		$_SESSION['errormsg'] = "Unknown user.<br>";
        // Redirects back to the login page.
        header('Location: index.php');
        exit;
	}
	else
    {
        echo "Checking password.<br>";
        if( password_verify( $_POST['password'], $fetched_result['password'] ) )
        {
            // Redirects to the main page.
            header('Location: main.php');
            exit;
        }
        else
        {
            $_SESSION['errormsg'] = "Invalid password.<br>";
			// Redirects back to the login page.
            header('Location: index.php');
            exit;
        }
    }

    ?>
    
    <head>
    </head>
    <body>
    </body>
</html>