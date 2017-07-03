<html>
    <?php session_start(); ?>
    <head>
        <title>Welcome to Microsoft Dota2 InHouse League</title>
    </head>
    <body>
    <div style="vertical-align:middle">
        <?php 
        if( !empty( $_SESSION['errormsg'] ) )
        {
            printf( "<font color='red'>%s</font>", $_SESSION['errormsg'] );
            $_SESSION['errormsg'] = '';
        }
        ?>
        <form action="loginvalidation.php" method="POST">
            <table align="center">
                <tr>
                    <th colspan="2">
                        Welcome to Microsoft Dota2 InHouse Client
                    </th>
                </tr>
                <tr>
                    <td>
                        User name:
                    </td>
                    <td>
                        <input name="username" type="text" value='<?php printf("%s", $_SESSION['username']);?>'>
                    </td>
                </tr>
                <tr>
                    <td>
                        Password:
                    </td>
                    <td>
                        <input name="password" type="password">
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <input type="submit" value="Login">
                    </td>
        </form>
        <form action="registration.php" method="POST">
                    <td align="left">
                        <input type="submit" value="Register">
                    </td>
        </form>
                </tr>
            </table>
    </div>
        
    </body>
</html>