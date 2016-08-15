<?php


/*==============Function Making Connection===============*/
	function Connection()
	{
		$mysql_host = "us-cdbr-azure-southcentral-f.cloudapp.net";
		$mysql_database = "muhafizdb";
		$mysql_user = "b894cd808ec87e";
		$mysql_password = "c6cd7754";

		$conn1 = new mysqli($mysql_host, $mysql_user, $mysql_password,$mysql_database);
		return $conn1; 
		
		
		/* $mysql_host = "localhost:3306";
		$mysql_database = "muhafizdatabase";
		$mysql_user = "root";
		$mysql_password = "";

		$conn = new mysqli($mysql_host, $mysql_user, $mysql_password,$mysql_database);
		return $conn; */



	}
/*======================================================*/	


function object_to_array($data){
	$result = [];
	while($row = $data->fetch_assoc()) {
		$result[] = $row;
	}
	return $result;
}

?>