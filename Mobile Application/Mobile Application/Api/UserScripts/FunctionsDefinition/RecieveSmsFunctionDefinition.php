<?php
/*==================Function for Sms Logs===============*/
    function GetSms_Logs($conn,$Mobileno)
    {

        $myquery = "SELECT Registration_Id,Name FROM RegistrationTable where Mobileno='$Mobileno'";


        $result = mysqli_query($conn, $myquery);
        $count=mysqli_num_rows($result);
        if($count==1){

            return $result;
        }
        else{

            return 0;
        }



    }
	/*===============Create table sms logs============*/
	function Create_TableSmsLogs($conn){
		
		 $sql = "CREATE TABLE SmsLogs(
                Smslog_Id INT(6) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                
                Name VARCHAR(200)  NOT NULL, 
                Reg_ID INT(6) UNSIGNED NOT NULL,
                Message VARCHAR(500) NULL,
                Sms_date TIMESTAMP NOT NULL,
					CONSTRAINT fk_Sms_Reg
                        FOREIGN KEY (Reg_ID) REFERENCES RegistrationTable(Registration_Id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                
            )";

        if (mysqli_query($conn, $sql)) {
            $result= "Table created successfully";
			return $result;
            
        } else {
           $result= "Error creating table: " . mysqli_error($conn);
		   return $result;
            
        }
		
		
		
	}
	/*===========Insert in Smslog===========*/	
	function Insert_InSmsLogs($conn,$name,$reg_ID,$message){
		$myquery = "INSERT INTO SmsLogs(Name,Reg_ID,Message,Sms_date) VALUES ('$name','$reg_ID','$message',CONVERT_TZ(CURRENT_TIMESTAMP,'-05:00','+00:00'));";
        if (mysqli_query($conn, $myquery))
        {   
            $result="Record entered  successfully";
            return $result;
        }
        else
        {
            $result= "".mysqli_error($conn);
            return $result;
        }
		
		
		
		
	}



?>