<?php
/*============including files of Function definition===================*/
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With, Content-Type, Origin, Authorization, Accept, Client-Security-Token, Accept-Encoding");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
include_once('ConnectionScript/Connectionscript.php');
include_once('FunctionsDefinition/ThreatReportDefinations.php');
/*========== Connection=======================================*/

$conn=Connection();

/*===========Check connection*===========================*/

if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);

}

/*=============condition if table already exists or not=============*/

$val = mysqli_query($conn, 'select 1 from `ThreatNotificationTable` LIMIT 1');
/*==============if table exists then====================*/

    if($val!=FALSE)
    {

        if ($_SERVER['REQUEST_METHOD'] === 'POST')
        {
            $_POST = json_decode(file_get_contents('php://input'), true);


            @$REGID = $_POST['REGID'];

            @$LEVELOFTHREAT = $_POST['LEVELOFTHREAT'];

            @$THREATSTATUS = $_POST['THREATSTATUS'];
            @$NAME= $_POST['NAME'];
            @$MOBILENO= $_POST['MOBILENO'];
            @$ROLE= $_POST['ROLE'];
            @$ORGANIZATIONNAME= $_POST['ORGANIZATIONNAME'];





            /*==========calling the insert function for inserting data in database================*/

            $result1 = Insert_In_ThreatNotificationTable($conn,$REGID,$LEVELOFTHREAT,$THREATSTATUS,$NAME,$MOBILENO,$ORGANIZATIONNAME);
            echo $result1;

            mysqli_close($conn);

        }

        else if($_SERVER['REQUEST_METHOD'] === 'GET')
        {
            @$Reg_Id = $_GET['REG_ID'];
           // echo $Reg_Id;

            $result=View_Reports($conn,$Reg_Id);
            if($result ===0){
                echo 0;
            }
            else{
              // header('Content-Type: application/json');
              //  for ($i=0; $i<count($result); $i++) {
                    echo json_encode($result);
                //}
              // echo $result;
            }


            mysqli_close($conn);

        }


    }
    else
    {
        //echo "dafaho jao";
        $query_result=CreateTable_Threats($conn);
		

        //echo $query_result;

        $_POST = json_decode(file_get_contents('php://input'), true);


            @$REGID = $_POST['REGID'];

            @$LEVELOFTHREAT = $_POST['LEVELOFTHREAT'];

            @$THREATSTATUS = $_POST['THREATSTATUS'];
            @$NAME= $_POST['NAME'];
            @$MOBILENO= $_POST['MOBILENO'];
            @$ROLE= $_POST['ROLE'];
            @$ORGANIZATIONNAME= $_POST['ORGANIZATIONNAME'];





            /*==========calling the insert function for inserting data in database================*/

            $result1 = Insert_In_ThreatNotificationTable($conn,$REGID,$LEVELOFTHREAT,$THREATSTATUS,$NAME,$MOBILENO,$ORGANIZATIONNAME);
            echo $result1;
        
        mysqli_close($conn);

    }

?>