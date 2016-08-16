	angular.module('Starting_Point.controllers', [])

			/*===========Start Controller============= */
		.controller('startCtrl', function ($scope, $ionicPopup, $location, $state, $localstorage) {
			/*=====Checking if the user is login======= */
		   if (typeof($localstorage.get('SESSION_USER')) === 'string')
			   $state.go('menu.UserHome');


			/*===definition of function wriiten behind the buttons====*/
			$scope.RedirecttoLogin = function () {
				$state.go('Login');
			}

			$scope.RedirecttoRegister = function () {
				$state.go('Registration');


			}

		})

		/*==============App controller====================*/
		.controller('menuCtrl', function($scope, $ionicSideMenuDelegate,$state,$ionicHistory) {

			/*======defination of function called when button is pressed=====*/
			$scope.ProfileSettings =
				function () {
				 $state.go('ProfileSetting');

				}

			$scope.ThreatReport=
				function () {
					$state.go('ThreatReport');

				}

			$scope.LogOut = function () {

					window.localStorage.clear();
					$ionicHistory.clearCache();
					$ionicHistory.clearCache().then(function(){
						$state.go('start');
					});

					//$state.go('start');

			}


		})

		/*==========================LOGIN CONTROLLER========================*/
		.controller('LoginCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $state, user, $localstorage, $ionicLoading) {

			$scope.data = {emailid:""};
			//Function called when back button is pressed

			$scope.redirecttohome = function () {
				$state.go('start');
			}
			//getting control of all labels value from constantsfile

			$scope.data.labels = user;

			//Function called when Login button is pressed
			$scope.login = function () {

				$ionicLoading.show({
					content: 'checking credentials',
					animation: 'fade-in',
					showBackdrop: true,
					maxWidth: 200,
					showDelay: 0
				});

				/*=========Request send when the user enter email and password======*/
				Ajaxfact.Request('GET', 'LoginandRegistrationscripts.php?names=' + $scope.data.emailid + '&password=' + $scope.data.password, "").success(function (data) {


					$ionicLoading.hide();
					if (data == "0") {
						var alertPopup = $ionicPopup.alert({
							title: 'Invalid Username Or Password'
						});

					}

					else {
						$scope.status=data.RegistrationStatus;
							if($scope.status=="Approved") {
								$localstorage.set('SESSION_USER', JSON.stringify(data));
								$state.go('menu.UserHome');

							}
							else{
								var alertPopup = $ionicPopup.alert({
									title: 'Your registration is not approved by admin'
								});

							}
					}
				}).error(function (data) {
					$ionicLoading.hide();
					var alertPopup = $ionicPopup.alert({
						title: 'Error',
						template: 'An error occured while processing your request. Kindly check your internet connection!' //+ data
					});
				});
			}
		})


		/*==========================REGISTRATION CONTROLLER========================*/
		.controller('RegisterCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $state, user, $ionicLoading) {

			$scope.data = {
				radioValue: ''
			};
			//variable for getting the selected value form dropdown

			var selectedvalue = -1;
			var status = "Pending";
			$scope.data.labels = user;
			//getting control of all labels value from constantsfile

			//Function called when value from dropdown is selected

			$scope.showSelectValue = function (mySelect) {
				selectedvalue = mySelect;

			}
			//Function called when back button is pressed

			$scope.redirecttohome =
				function () {
					$state.go('start');

				}


			$scope.Register = function () {


				$ionicLoading.show({
					content: 'Loading',
					animation: 'fade-in',
					showBackdrop: true,
					maxWidth: 200,
					showDelay: 0
				});


				var date = new Date();
				var regdate = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);

				if ($scope.data.radioValue == 'Freelance') {
					$scope.data.Organization = "Freelancer";
				}


				var data = {
					FNAME: $scope.data.FName,
					EMAILID: $scope.data.emailid,
					RADIOVALUE: $scope.data.radioValue,
					ORGANIZATION: $scope.data.Organization,
					PHONENO: $scope.data.phoneno,
					STATIONEDAT: $scope.data.StationedAt,
					SELECTEDVALUE: selectedvalue,
					PASSWORD: $scope.data.password,
					REGSTATUS: "Pending",
					REGDATE: regdate
				}

				/*====Sending data to store in database=====*/

				Ajaxfact.Request('POST', 'LoginandRegistrationscripts.php', data).success(function (data) {
					$ionicLoading.hide();

					var resdata = data;
					if (resdata.match("Mobileno")) {
						var alertPopup = $ionicPopup.alert({
						  //  title: 'Success',
							template:'Account with this mobile already exists.'
						});

					}
					else if (resdata.match("Email")) {
						var alertPopup = $ionicPopup.alert({
						   // title: 'Success',
							template:'Account with this email already exists.'
						});

					}
					else if (resdata.match("inserted successfully")) {
						var alertPopup = $ionicPopup.alert({
							//title: 'Success',
							template:'Thank you. Your information has been submitted.'
						});
						$state.go('pendingRegistration');

					}
					else {

						var alertPopup = $ionicPopup.alert({
							title: 'Invalid Credentials',
							template: 'could not received your data successfully!' + data
						});
					}

				}).error(function (data) {
					$ionicLoading.hide();
					var alertPopup = $ionicPopup.alert({
						 title: 'Error',
						template: 'An error occured while processing your request. Kindly check your internet connection!' //+ data
					});
				});


			}


		})
		/*==========================Pending Registration CONTROLLER========================*/

		.controller('pendingRegistrationCtrl', function ($scope, $state, user) {
			$scope.data = {};
			$scope.data.labels = user;
			/*====redirect to start page=====*/
			$scope.redirecttohome =
				function () {
					$state.go('start');

				}


		})
		/*==========================UserHome CONTROLLER========================*/

		.controller('UserHomeCtrl', function ($scope, $http, Ajaxfact,ConnectivityMonitor, $ionicPopup, $ionicLoading,$cordovaNetwork,$stateParams, $state,$rootScope, user, $localstorage, $ionicHistory) {

			$scope.data = {};
			$scope.data.display = true;
			var ooflinelocalstorage
			$scope.currDate = new Date();
			var datee=$scope.currDate.getFullYear() + '-' + ('0' + ($scope.currDate.getMonth() + 1)).slice(-2) + '-' + ('0' + $scope.currDate.getDate()).slice(-2)+  '  ' + ('0' + ($scope.currDate.getHours())).slice(-2)+  ':' + ('0' + ($scope.currDate.getMinutes())).slice(-2)+':' + ('0' + ($scope.currDate.getSeconds())).slice(-2);
			//$scope.date-format-hhmmsstt = $filter('date')(new Date(), 'hh:mm:ss a');
			//var datee=$scope.date-format-hhmmsstt ;

			/*Retrieving data from session object*/
			var User = JSON.parse($localstorage.get('SESSION_USER'));
			$scope.data.Name = User.Name;
			$scope.data.Mobileno = User.Mobileno;
			$scope.data.workat = User.OrganizationName;
			$scope.data.reg_id = User.Registration_Id;
			$scope.data.Email = User.Email;
			$scope.data.Address = User.Address;
			$scope.data.Role = User.Role;
			$scope.data.Password = User.Password;

			/*====getting control over the labels names from constant file*/
			$scope.data.labels = user;

			/*=========Definition of function written behind the buttons=====*/
			
			/*=========Function for generating random numbers=========*/
			function getuid()
			{
				function s4() 
				{
				
				return Math.floor((1 + Math.random()) * 0x10000)

				}
			 return s4() + s4();
			}
			

			$scope.LogOut =
				function () {


					window.localStorage.clear();
					$ionicHistory.clearCache();
					$ionicHistory.clearCache().then(function(){
						$state.go('start');
					});

					//$state.go('start');

				}

			$scope.PanicAttack=
				function() {
					

					var data = {
						REGID: $scope.data.reg_id,
						NAME: $scope.data.Name,
						MOBILENO: $scope.data.Mobileno,
						ROLE:$scope.data.Role,
						ORGANIZATIONNAME:$scope.data.workat,
						Regdate:datee
					}
					
					
			
					$ionicLoading.show({
						content: 'Loading',
						animation: 'fade-in',
						showBackdrop: true,
						maxWidth: 200,
						showDelay: 0
					});
					
					
					

					/*====Check if the device is ofline=======*/
					if (ConnectivityMonitor.isOffline()) {

						$ionicLoading.hide();
						
						/*==========Code for sending message through the user sim=====================*/
						var randomno = getuid();
						var sendto = "03425063376";//admin no
						var textmsg = "$$@@##"+randomno;
						
						
						
						var options = {
						replaceLineBreaks: false, // true to replace \n by a new line, false by default
							android: {
								intent: 'INTENT'  // send SMS with the native android SMS messaging
								//intent: '' // send SMS without open any other app
							}
						};

		   
			
			
					var success = function (hasPermission) { 
						if (hasPermission) {
							sms.send(sendto,textmsg, options);
							alert('Sending SMS to Muhafiz team');
							
						}
						else {
							alert('Please grant SMS permission to Muhafiz application to send panic alerts using SMS.');
							sms.requestPermission();
							// show a helpful message to explain why you need to require the permission to send a SMS
							// read http://developer.android.com/training/permissions/requesting.html#explain for more best practices
						}
					};
					var error = function (e) { alert('An error occurred while requesting sms permission. Please try again. \n' + e); };
					sms.hasPermission(success, error);
						
						
						
						
						
						
						


					}
					/*----Checking f the deice is online-----*/
					if (ConnectivityMonitor.isOnline()) {
						
						Ajaxfact.Request('POST', 'PanicAttackScripts.php', data).success(function (data) {
							$ionicLoading.hide();
							var alertPopup = $ionicPopup.alert({
								//title: 'msg',
								template: 'SMS sent to Muhafiz team.' //+data
							});

						}).error(function (data) {
							$ionicLoading.hide();
							var alertPopup = $ionicPopup.alert({
								title: 'Error',
								template: 'An error occurred while processing panic request.'
							});
						}); 
						


					}
				}


			 /*to show previous reports in table*/
			$scope.PreviousReports =
				function () {

					$scope.ThreatReports = [];
					$scope.data.display = false;

					$ionicLoading.show({
						content: 'Loading',
						animation: 'fade-in',
						showBackdrop: true,
						maxWidth: 200,
						showDelay: 0
					});
					
					Ajaxfact.Request('GET', 'ThreatReportScripts.php?REG_ID=' + $scope.data.reg_id, "").success(function (data) {
						$ionicLoading.hide();
						if (data == "0") {
							var alertPopup = $ionicPopup.alert({
								title: "Status",
								template: 'No threats have been reported yet.'


							});

						}

						else {


							$scope.ThreatReports = data;


						}
					}).error(function (data) {
						$ionicLoading.hide();
						var alertPopup = $ionicPopup.alert({
							title: 'Error',
						template: 'An error occured while processing your request. Kindly check your internet connection!' //+ data
						});
					});
				}


		})


		/*==========================ProfileSetting CONTROLLER========================*/

		.controller('ProfileSettingCtrl', function ($scope, $http, Ajaxfact, $ionicPopup, $stateParams, $state, user, $localstorage, $ionicLoading) {

			$scope.data = {};
			$scope.data.labels = user;
		/*======retriving data from storage=========*/
			var User = JSON.parse($localstorage.get('SESSION_USER'));
			$scope.data.FName = User.Name;
			$scope.data.phoneno = User.Mobileno;
			$scope.data.Organization = User.OrganizationName;
			$scope.data.reg_id = User.Registration_Id;
			$scope.data.emailid = User.Email;
			$scope.data.Role = User.Role;
			$scope.data.StationedAt = User.StationedAt;


			/*==Function definations==*/


			$scope.Home = function () {

				$state.go('menu.UserHome');
			}
			/*===Sending the request to update profile*/
			$scope.SaveInfo = function () {
				


			$scope.currDate = new Date();
			var datee=$scope.currDate.getFullYear() + '-' + ('0' + ($scope.currDate.getMonth() + 1)).slice(-2) + '-' + ('0' + $scope.currDate.getDate()).slice(-2)+  '  ' + ('0' + ($scope.currDate.getHours())).slice(-2)+  ':' + ('0' + ($scope.currDate.getMinutes())).slice(-2)+':' + ('0' + ($scope.currDate.getSeconds())).slice(-2);
			
			/*===retrieving data from session===*/
				var data={
					REGID: $scope.data.reg_id,
					STATUS:"pending",
					REGDATE: datee,
					EMAIL:$scope.data.emailid
				}
				 
				$ionicLoading.show({
					content: 'Loading',
					animation: 'fade-in',
					showBackdrop: true,
					maxWidth: 200,
					showDelay: 0
				});

				Ajaxfact.Request('POST', 'ProfileSettingsScripts.php', data).success(function (data) {
					$ionicLoading.hide();
					var alertPopup = $ionicPopup.alert({
					   // title: 'Success !',
						template: 'Your profile update request has been sent to Muhafiz team. The team will contact you shortly.' //+data
					});
					$state.go('menu.UserHome');

				}).error(function (data) {
					$ionicLoading.hide();
					var alertPopup = $ionicPopup.alert({
						title: 'Error',
						template: 'An error occured while processing your request. Kindly check your internet connection!' //+ data
					});
				});


			}


		})
		/*==========================ThreatReportCONTROLLER========================*/
		.controller('ThreatReportCtrl', function ($scope, $state, $ionicPopup, Ajaxfact, $stateParams, user, $localstorage, $ionicLoading) {
			//Function called when back button is pressed

			$scope.data = {};
			//getting control of all labels value from constantsfile
			$scope.data.labels = user;
			$scope.currDate = new Date();
			var datee=$scope.currDate.getFullYear() + '-' + ('0' + ($scope.currDate.getMonth() + 1)).slice(-2) + '-' + ('0' + $scope.currDate.getDate()).slice(-2)+  '  ' + ('0' + ($scope.currDate.getHours())).slice(-2)+  ':' + ('0' + ($scope.currDate.getMinutes())).slice(-2)+':' + ('0' + ($scope.currDate.getSeconds())).slice(-2);
			
			/*===retrieving data from session===*/
			var User = JSON.parse($localstorage.get('SESSION_USER'));
			$scope.data.Name = User.Name;
			$scope.data.Mobileno = User.Mobileno;
			$scope.data.workat = User.OrganizationName;
			$scope.data.reg_id = User.Registration_Id;
			$scope.data.Email = User.Email;
			$scope.data.Role = User.Role;


			//Function called when back button is pressed
			$scope.Home = function () {

				$state.go('menu.UserHome');
			}

	/*========Request to send threat if any=====*/
			$scope.SubmitThreat =
				function () {

					$ionicLoading.show({
						content: 'processing',
						animation: 'fade-in',
						showBackdrop: true,
						maxWidth: 200,
						showDelay: 0
					});

					var data = {
						REGID: $scope.data.reg_id,
						LEVELOFTHREAT: $scope.data.Threatlevel,
						THREATSTATUS: $scope.data.Threatstatus,
						NAME: $scope.data.Name,
						MOBILENO: $scope.data.Mobileno,
						ROLE:$scope.data.Role,
						ORGANIZATIONNAME:$scope.data.workat,
						ThreatDate:datee
					}


					Ajaxfact.Request('POST', 'ThreatReportScripts.php', data)
						.success(function (data) {
							$ionicLoading.hide();

							var alertPopup = $ionicPopup.alert({
								//title: 'Success', //+ data
								template:'Threat report submitted.' //+data
							});

						$state.go('menu.UserHome');

						}).error(function (data) {
						$ionicLoading.hide();
						var alertPopup = $ionicPopup.alert({
							title: 'Error',
						template: 'An error occured while processing your request. Kindly check your internet connection!' //+ data
						});

					});



				}


		});

