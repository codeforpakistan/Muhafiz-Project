<?php
//header("Access-Control-Allow-Origin: *");
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
include_once('ConnectionScript/Connectionscript.php');
include_once('FunctionsDefinition/RecieveSmsFunctionDefinition.php');
include_once('FunctionsDefinition/PanicDefinitions.php');

/*========== Connection=======================================*/

$conn=Connection();

		// Check connection

if ($conn->connect_error)
{
    die("Connection failed: " . $conn->connect_error);




}
else{
	  //echo "bue";
		if($_SERVER['REQUEST_METHOD'] === 'POST'){
			
	//	$_POST = json_decode(file_get_contents('php://input'), true);
			
			echo json_encode($_POST['BODY']);
		}
		
		else if($_SERVER['REQUEST_METHOD'] === 'GET'){
			
			@$mobileno = $_GET['mobileno'];
			@$message =   $_GET['message'];
			$res=GetSms_Logs($conn,$mobileno);
			 
			if($res ===0)
			{
				echo 0;
            }
       
			else{
				
				//header('Content-Type: application/json');
				 $data =mysqli_fetch_assoc($res);
				 $val = mysqli_query($conn, 'select 1 from `SmsLogs` LIMIT 1');
				 if($val==FALSE){
					 $result2=Create_TableSmsLogs($conn);
					 if($result2=="Table created successfully"){
							$result=Insert_InSmsLogs($conn,$data["Name"],$data["Registration_Id"],$message);
							$result1 = Insert_In_PanicAttack($conn,$data["Registration_Id"]);
							echo json_encode($result1);
					 }
					 
				 }
				 else
				 {
					$result=Insert_InSmsLogs($conn,$data["Name"],$data["Registration_Id"],$message);
					$result1 = Insert_In_PanicAttack($conn,$data["Registration_Id"]);
					echo json_encode($result1);
				 }
				

			}
		//	echo json_encode($_GET['mobileno']);
			
			
		}
	
	







}



?>