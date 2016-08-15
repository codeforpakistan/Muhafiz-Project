<?php
/*============including files of Function definition===================*/
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With, Content-Type, Origin, Authorization, Accept, Client-Security-Token, Accept-Encoding");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
include_once('ConnectionScript/Connectionscript.php');
include_once('FunctionsDefinition/LoginandRegistrationDefinations.php');

/*========== Connection=======================================*/

$conn=Connection();

/*===========Check connection*===========================*/

if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);

}

/*=============condition if table already exists or not=============*/

$val = mysqli_query($conn, 'select 1 from `RegistrationTable` LIMIT 1');

/*==============if table exists then====================*/

if($val!=FALSE)
{

    if ($_SERVER['REQUEST_METHOD'] === 'POST')
    {
        $_POST = json_decode(file_get_contents('php://input'), true);


        @$Name = $_POST['FNAME'];

        @$WorkedAt = $_POST['RADIOVALUE'];

        @$OrganizationName = $_POST['ORGANIZATION'];

        @$Mobileno =$_POST['PHONENO'];

        @$Email =$_POST['EMAILID'];

        @$StationedAt = $_POST['STATIONEDAT'];

        @$Role = $_POST['SELECTEDVALUE'];

        @$Password = $_POST['PASSWORD'];

        $Password= md5($Password);

        @$REGStatus =$_POST['REGSTATUS'];

        @$reg_date = $_POST['REGDATE'];



        /*==========calling the insert function for inserting data in database================*/

        $result1 = Insert_In_RegistrationTable($conn, $Email, $Name, $WorkedAt, $OrganizationName, $Mobileno, $StationedAt, $Role, $Password,$REGStatus);
        echo $result1;
        mysqli_close($conn);

    }

    else if($_SERVER['REQUEST_METHOD'] === 'GET')
    {
        @$email = $_GET['names'];
        @$passwordd = md5($_GET['password']);
        $result=Login_User($conn,$email,$passwordd);
        if($result ===0){
            echo 0;
        }
        else{
            header('Content-Type: application/json');
            echo json_encode(mysqli_fetch_assoc($result));

        }


        mysqli_close($conn);

    }

    else if ($_SERVER["REQUEST_METHOD"] === 'PUT')


    {     $_PUT = json_decode(file_get_contents('php://input'), true);

            @$Name = $_PUT['FNAME'];
          //  echo $Name;

            @$OrganizationName = $_PUT['ORGANIZATION'];
          //  echo $OrganizationName;
            @$Mobileno =$_PUT['PHONENO'];
        //echo $Mobileno;

            @$Email =$_PUT['EMAILID'];
      // echo $Email;
            @$StationedAt = $_PUT['STATIONEDAT'];
      //  echo $StationedAt;
            @$Role = $_PUT['SELECTEDVALUE'];
      //  echo $Role;
            @$Password = $_PUT['PASSWORD'];


            //$Password= md5($Password);

            @$REG_id =$_PUT['REG_ID'];

        if(is_null($Password)){


            $result1 = Update_In_RegistrationTable2($conn, $Email, $Name,  $OrganizationName, $Mobileno, $StationedAt, $Role,$REG_id);

             echo $result1;
              mysqli_close($conn);

        }
        else {
            $Password= md5($Password);

            /*==========calling the update function for updating data in database================*/

             $result1 = Update_In_RegistrationTable($conn, $Email, $Name,  $OrganizationName, $Mobileno, $StationedAt, $Role, $Password,$REG_id);

            echo $result1;
            mysqli_close($conn);

        }


    }


}
else
{
    //echo "dafaho jao";
    $query_result=CreateTable_Registration($conn);
    //echo $query_result;

    $_POST = json_decode(file_get_contents('php://input'), true);


    @$Name = $_POST['FNAME'];

    @$WorkedAt = $_POST['RADIOVALUE'];

    @$OrganizationName = $_POST['ORGANIZATION'];

    @$Mobileno =$_POST['PHONENO'];

    @$Email =$_POST['EMAILID'];

    @$StationedAt = $_POST['STATIONEDAT'];

    @$Role = $_POST['SELECTEDVALUE'];

    @$Password = $_POST['PASSWORD'];

    $Password= md5($Password);

    @$REGStatus =$_POST['REGSTATUS'];

    @$reg_date = $_POST['REGDATE'];



    /*==========calling the insert function for inserting data in database================*/

    $result1 = Insert_In_RegistrationTable($conn, $Email, $Name, $WorkedAt, $OrganizationName, $Mobileno, $StationedAt, $Role, $Password,$REGStatus);
    echo $result1;
    mysqli_close($conn);

}

?>