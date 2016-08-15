<?php
include_once('PHPMailer-master/PHPMailerAutoload.php');
/*================TABLE FOR STORING Request for Profile setting DATA====================*/

function CreateTable_ProfileSettings($conn)
{

    $sql = "CREATE TABLE ProfileSettings(
                    ProfileSettings_Id INT(6) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                    RegistrationId INT(6) UNSIGNED NOT NULL,
                    Status VARCHAR(100) NOT NULL,
                    ProfileSettings_date TIMESTAMP NOT NULL , 
                    
                        CONSTRAINT fk_Settings
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
 /*==============***Function for sending emails============================*/
    function smtpmailer($to, $from, $from_name, $subject, $body) {
        global $error;
        $mail = new PHPMailer();  // create a new object
        $mail->IsSMTP(); // enable SMTP
        $mail->SMTPDebug = 0;  // debugging: 1 = errors and messages, 2 = messages only
        $mail->SMTPAuth = true;  // authentication enabled
        $mail->SMTPSecure = 'tls'; // secure transfer enabled REQUIRED for GMail
        $mail->Host = 'smtp.gmail.com';
        $mail->Port = 587;
        $mail->Username = 'muhafizgroup123@gmail.com';
        $mail->Password = 'muhafiz123';
        $mail->SetFrom($from, $from_name);
        $mail->Subject = $subject;
        $mail->Body = $body;
        $mail->AddAddress($to);
        if(!$mail->Send()) {
            $error = 'Mail error: '.$mail->ErrorInfo;
            return false;
        } else {
            $error = 'Message sent!';
            return true;
        }
    }

/*=======Insert data in profile settings table======*/
function Insert_In_ProfileSettings($conn,$Regid,$status,$Email)
{
	//$result;
    $myquery = "INSERT INTO ProfileSettings(RegistrationId,Status,ProfileSettings_date) VALUES ('$Regid','$status',CURRENT_TIMESTAMP);";
    if (mysqli_query($conn, $myquery))
    {
			$sub="Profile Settings Pending";
            $body="Your Request to update your profile has been sent to Muhafiz team ,you will be called to update your Profile";
            
           if (smtpmailer($Email, 'muhafizgroup123@gmail.com', 'MuhafizAdmin', $sub, $body)) {
               
                $result="inserted successfully";
				return $result;
                
            }
            if (!empty($error)) {
				
                $result=$error;
				return $result;
				
            }
			
			return $result;
        
        
    }
    else
    {
        $result= "".mysqli_error($conn);
        return $result;
    }
}



?>