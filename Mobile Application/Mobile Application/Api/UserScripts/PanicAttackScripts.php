<?php
/*============including files of Function definition===================*/
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Headers: X-Requested-With, Content-Type, Origin, Authorization, Accept, Client-Security-Token, Accept-Encoding");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
include_once('ConnectionScript/Connectionscript.php');
include_once('FunctionsDefinition/PanicDefinitions.php');
/*========== Connection=======================================*/

$conn=Connection();

/*===========Check connection*===========================*/

if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);

}

/*=============condition if table already exists or not=============*/

$val = mysqli_query($conn, 'select 1 from `PanicAttack` LIMIT 1');
/*==============if table exists then====================*/

if($val!=FALSE)
{

    if ($_SERVER['REQUEST_METHOD'] === 'POST')
    {
        $_POST = json_decode(file_get_contents('php://input'), true);


        @$REGID = $_POST['REGID'];









        /*==========calling the insert function for inserting data in database================*/

        $result1 = Insert_In_PanicAttack($conn,$REGID);
        echo $result1;
        mysqli_close($conn);

    }

    else if($_SERVER['REQUEST_METHOD'] === 'GET')
    {
       /* @$Reg_Id = $_GET['REG_ID'];
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


        mysqli_close($conn);*/

    }


}
else
{
    
    $query_result=CreateTable_PanicAttack($conn);
    

    $_POST = json_decode(file_get_contents('php://input'), true);


    @$REGID = $_POST['REGID'];

    @$Panic_date = $_POST['REGDATE'];







    /*==========calling the insert function for inserting data in database================*/

    $result1 = Insert_In_PanicAttack($conn,$REGID);
    echo $result1;
    mysqli_close($conn);


}

?>