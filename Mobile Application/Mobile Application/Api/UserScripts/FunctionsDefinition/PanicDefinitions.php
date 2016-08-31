<?php
include_once('UfoneBusinessSMSAPI-master/UfoneBusinessSMSAPI-master/UfoneBusinessSMS.php');
/*================TABLE FOR STORING PANIC ATTACK DATA====================*/

function CreateTable_PanicAttack($conn)
{

    $sql = "CREATE TABLE PanicAttack(
                    PanicAttack_Id INT(6) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                    RegistrationId INT(6) UNSIGNED NOT NULL,
                    panic_date TIMESTAMP NOT NULL , 
					seen_status VARCHAR(50) NOT NULL ,
                    
                        CONSTRAINT fk_Attack
                        FOREIGN KEY (RegistrationId) REFERENCES RegistrationTable(Registration_Id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    
                )";

    if (mysqli_query($conn, $sql)) {
        $result="Table created successfully";
        return $result;
    } else {
        $result="Error creating table: " . mysqli_error($conn);
        return $result;
    }


}





/*=======Insert data in witness table======*/
function Insert_In_PanicAttack($conn,$Regid)
{
	  $adminno="923355243732";
    $myquery = "INSERT INTO PanicAttack(RegistrationId,panic_date,seen_status) VALUES ('$Regid',CONVERT_TZ(CURRENT_TIMESTAMP,'-05:00','+00:00'),'NotSeen');";
    if (mysqli_query($conn, $myquery))
    {
      date_default_timezone_set("Asia/Karachi");
      $date = date("Y-m-d h:i:sa", strtotime("+1 hours"));
			$randomno = rand(1,10000);
      $message="$$@@##".$randomno;
			
			$sms = new UfoneBusinessSMS("03359646370", "B-SMS", "321321");
			$result = $sms->sendSMS($adminno, $message);
			$result1="inserted successfully";
			return $result1;
    }
    else
    {
        $result= "".mysqli_error($conn);
        return $result;
    }
}



?>