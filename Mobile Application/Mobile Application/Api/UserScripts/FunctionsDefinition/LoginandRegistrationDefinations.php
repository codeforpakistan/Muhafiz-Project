<?php
include_once('PHPMailer-master/PHPMailerAutoload.php');
include_once('SMS/ViaNettSMS.php');
/*================TABLE FOR STORING REGISTERED USER DATA====================*/

    function CreateTable_Registration($conn)
    {

        $sql = "CREATE TABLE RegistrationTable(
                Registration_Id INT(6) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                
                Email VARCHAR(200)  UNIQUE NOT NULL, 
                Name VARCHAR(500) NOT NULL,
                WorkedAt VARCHAR(100) NOT NULL,
                OrganizationName VARCHAR(500) NULL,
                Mobileno VARCHAR (15) UNIQUE NOT NULL,
                StationedAt VARCHAR(1000) NOT NULL,
                Role VARCHAR(100) NOT NULL,
                Password VARCHAR(900) NOT NULL,
                RegistrationStatus VARCHAR(50) ,
                reg_date TIMESTAMP NOT NULL
                
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


                
/*========SmS Send Func=========*/
function SendSms($MessageBody){
    $adminnumber="923425063376";
    $ch = curl_init();

// set URL and other appropriate options
    curl_setopt($ch, CURLOPT_URL, "http://api.bizsms.pk/api-send-branded-sms.aspx?username=d-sales-ay@bizsms.pk&pass=d3mosal3s45786**&text=".urlencode($MessageBody)."&masking=Demo&destinationnum=".urlencode($adminnumber)."&language=English");
    curl_setopt($ch, CURLOPT_HEADER, false);

// grab URL and pass it to the browser
    curl_exec($ch);

// close cURL resource, and free up system resources
    curl_close($ch);


}

/*============ INSERT DATA IN REGISTARTION TABLE======================*/
    function Insert_In_RegistrationTable($conn, $Email, $Name, $WorkedAt, $OrganizationName, $Mobileno, $StationedAt, $Role, $Password,$regstatus)
    {

        $myquery = "INSERT INTO RegistrationTable(Email,Name,WorkedAt, OrganizationName, Mobileno,StationedAt,
			Role, Password,RegistrationStatus, reg_date) VALUES ('$Email','$Name','$WorkedAt',
			'$OrganizationName', '$Mobileno','$StationedAt','$Role', '$Password','$regstatus',
			CURRENT_TIMESTAMP);";
        if (mysqli_query($conn, $myquery))

        {
            date_default_timezone_set("Asia/Dili");

            $date = date("Y-m-d h:i:sa", strtotime("-4 hours"));

            $message="Registration Request from: ".$Name.", OrganizationName: ".$OrganizationName.", Role: ".$Role.", DateandTime : ".$date;
            //SendSms($message);
            $sub="Registration Pending";
            $body="Thank you for signing up. You will be notified when your registration request has been approved.";;
            
            if (smtpmailer($Email, 'muhafizgroup123@gmail.com', 'MuhafizAdmin', $sub, $body)) {
               // echo "senddddddddd";
                $result="inserted successfully";
                // do something
            }
            if (!empty($error)) {
                $result=$error;
				//echo $result;
            }
            
            return $result;
        }
        else
        {
            $result= "".mysqli_error($conn);
            return $result;
        }
    }


/*==================Function for Login===============*/
    function Login_User($conn,$Email,$Password)
    {

        $myquery = "SELECT * FROM RegistrationTable where Email='$Email'and Password='$Password'";


        $result = mysqli_query($conn, $myquery);
        $count=mysqli_num_rows($result);
        if($count==1){

            return $result;
        }
        else{

            return 0;
        }



    }





?>