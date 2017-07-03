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
            $link = mysql_connect('localhost','WebClient','MSFTDota2InHouse')
                or die( 'Could not connect: ' . mysql_error() );
            echo "Connected to database. <br>";
            mysql_select_db('userinfo')
                or die( 'Could not select database. <br>' );
            
            $query = sprintf("SELECT * FROM `basicinfo` WHERE `username` LIKE '%s'", $username);
            $result = mysql_query( $query )
                or die( 'Query failed: ' . mysql_error() );
            
            $fetched_result = mysql_fetch_array( $result, MYSQL_ASSOC );
            
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
            
            $password_hased = password_hash($password, PASSWORD_DEFAULT);
            
            $query =  sprintf( "INSERT INTO `userinfo`.`basicinfo` (`username`, `password`, `steamprofile`, `1stPos`, `2ndPos`) VALUES ( '%s', '%s', '%s', '%s', '%s');",
                                                                    $username, $password_hased, $steamprofile, $pos1, $pos2 );
            $result = mysql_query( $query )
                or die( 'Query failed: ' . mysql_error() );
            
            if( $result )
            {
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