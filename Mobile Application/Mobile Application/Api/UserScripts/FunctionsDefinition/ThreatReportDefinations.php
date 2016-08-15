<?php
include_once('UfoneBusinessSMSAPI-master/UfoneBusinessSMSAPI-master/UfoneBusinessSMS.php');
/*================TABLE FOR STORING THREAT NOTIFICATIONS DATA====================*/

    function CreateTable_Threats($conn)
    {

       $sql = "CREATE TABLE ThreatNotificationTable(
                    ThreatReportNotification_Id INT(6)  UNSIGNED AUTO_INCREMENT UNIQUE PRIMARY KEY,
                    RegistrationId INT(6) UNSIGNED NOT NULL,
                    Level_of_Threat VARCHAR(20) NOT NULL, 
                    Threat_status VARCHAR(5) NOT NULL,
                    ReportDate TIMESTAMP NOT NULL,
                    ProcessThreat_status VARCHAR(20) NOT NULL,
                        CONSTRAINT fk_ThreatNoti
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

   



    /*=======Insert data in ThreatNotificationTable table======*/
    function Insert_In_ThreatNotificationTable($conn,$Regid,$Threatlevel,$Threatstatus,$Name,$Mobileno,$Organizationname)
    {
		$adminno="923425063376";

        $myquery = "INSERT INTO ThreatNotificationTable(RegistrationId,Level_of_Threat,Threat_status,ReportDate,ProcessThreat_status) VALUES ('$Regid','$Threatlevel','$Threatstatus',CURRENT_TIMESTAMP,'Pending');";
        if (mysqli_query($conn, $myquery))
        {   date_default_timezone_set("Asia/Dili");

            $date = date("Y-m-d h:i:sa", strtotime("-16 hours"));
            $message="Threat Report from :".$Name.", Registration Id: ".$Regid.", Mobileno : ".$Mobileno.", OrganizatioName: ".$Organizationname.", DateandTime : ".$date;
			
			$sms = new UfoneBusinessSMS("03359646370", "B-SMS", "321321");
			$result = $sms->sendSMS($adminno, $message);
			
			return $result;
        }
        else
        {
            $result= "".mysqli_error($conn);
            return $result;
        }
    }

    /*==========Table of parent thread===========*/
/*================TABLE FOR STORING PARENT THREAT DATA====================*/

function CreateTable_ParentThreats($conn)
{

   $sql = "CREATE TABLE ThreatReportTable(
                    ThreatReport_Id INT(6) UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                    ThreatReportNotificationId INT(6) UNSIGNED UNIQUE NOT NULL,
                    ReportDate TIMESTAMP NOT NULL,
                    Level_of_Threat VARCHAR(20) NOT NULL, 
                       CONSTRAINT fk_Notification
                        FOREIGN KEY (ThreatReportNotificationId) REFERENCES ThreatNotificationTable(ThreatReportNotification_Id)
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
function Insert_In_ThreatReportTable($conn,$Regid,$Threatlevel)
{

    $myquery = "INSERT INTO ThreatReportTable(RegistrationId,ReportDate,Level_of_Threat) VALUES ('$Regid',CURRENT_TIMESTAMP,'$Threatlevel');";
    if (mysqli_query($conn, $myquery))
    {
        $result="inserted successfully";
        return $result;
    }
    else
    {
        $result= "".mysqli_error($conn);
        return $result;
    }
}






    /*==================Function for Login===============*/
    function View_Reports($conn,$Reg_id)
    {

        $myquery = "SELECT * FROM ThreatReportTable,ThreatNotificationTable where ThreatReportNotificationId=ThreatReportNotification_Id and RegistrationId='$Reg_id'";
        $result = mysqli_query($conn, $myquery);
        if (mysqli_query($conn, $myquery)) {

            return object_to_array($result);
        }
        else {

            return 0;

        }
       // $count=mysqli_num_rows($result);




    }

?>