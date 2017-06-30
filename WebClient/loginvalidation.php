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
    
    $link = mysql_connect($_SESSION['db_server'],$_SESSION['db_username'],$_SESSION['db_password'])
        or die( 'Could not connect: ' . mysql_error() );
    $_SESSION['db_link'] = $link;
    echo "Connected to database.<br>";
    mysql_select_db('userinfo')
        or die( 'Could not select database. <br>' );
    
    echo "Checking username.<br>";
    $query = sprintf("SELECT * FROM `basicinfo` WHERE `username` LIKE '%s'", $username);
    $result = mysql_query( $query )
        or die( 'Query failed: ' . mysql_error() );
        
    
    $fetched_result = mysql_fetch_array( $result, MYSQL_ASSOC );
    
    if( $fetched_result == FALSE )
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
            echo "Valid password. <br>";
            // Redirects back to the main page.
            header('Location: main.php');
            exit;
        }
        else
        {
            echo "Invalid password.<br>";
            $_SESSION['errormsg'] = "Invalid password.<br>";
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